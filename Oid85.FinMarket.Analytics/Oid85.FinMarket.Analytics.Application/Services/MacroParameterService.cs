using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Storage.Core.Requests;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class MacroParameterService(
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient) 
        : IMacroParameterService
    {
        public async Task<CreateOrUpdateAnalyticMacroParameterResponse> CreateOrUpdateAnalyticMacroParameterAsync(CreateOrUpdateAnalyticMacroParameterRequest request)
        {
            await finMarketStorageServiceApiClient.CreateOrUpdateConsumerPriceIndexChangeAsync(
                new CreateOrUpdateConsumerPriceIndexChangeRequest { Date = request.Date, Value = request.ConsumerPriceIndexChange });

            await finMarketStorageServiceApiClient.CreateOrUpdateMonetaryAggregateAsync(
                new CreateOrUpdateMonetaryAggregateRequest { Date = request.Date, M0 = request.M0, M1 = request.M1, M2 = request.M2, M2X = request.M2X });

            await finMarketStorageServiceApiClient.CreateOrUpdateKeyRateAsync(
                new CreateOrUpdateKeyRateRequest { Date = request.Date, Value = request.KeyRate });

            return new CreateOrUpdateAnalyticMacroParameterResponse();
        }

        public async Task<GetAnalyticMacroParameterListResponse> GetAnalyticMacroParameterListAsync(GetAnalyticMacroParameterListRequest request)
        {
            var consumerPriceIndexChanges = (await finMarketStorageServiceApiClient.GetConsumerPriceIndexChangeListAsync(new())).Result.ConsumerPriceIndexChanges.OrderBy(x => x.Date).ToList();
            var monetaryAggregates = (await finMarketStorageServiceApiClient.GetMonetaryAggregateListAsync(new())).Result.MonetaryAggregates.OrderBy(x => x.Date).ToList();
            var keyRates = (await finMarketStorageServiceApiClient.GetKeyRateListAsync(new())).Result.KeyRates.OrderBy(x => x.Date).ToList();
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));
            var to = DateOnly.FromDateTime(DateTime.Today);
            var moexIndexCandles = (await finMarketStorageServiceApiClient.GetCandleListAsync(new GetCandleListRequest { Ticker = KnownIndexTickers.IMOEX, From = from, To = to })).Result.Candles.OrderBy(x => x.Date).ToList();
            var rgbiIndexCandles = (await finMarketStorageServiceApiClient.GetCandleListAsync(new GetCandleListRequest { Ticker = KnownIndexTickers.RGBI, From = from, To = to })).Result.Candles.OrderBy(x => x.Date).ToList();
            var dates = DateUtils.GetMonthDates(from, to);
            
            var macroParameterItems = new List<GetAnalyticMacroParameterItemListResponse>();

            foreach (var date in dates)
            {
                var macroParameterItem = new GetAnalyticMacroParameterItemListResponse
                {
                    Date = date
                };

                var consumerPriceIndexChange = consumerPriceIndexChanges.FirstOrDefault(x => x.Date == date);
                var monetaryAggregate = monetaryAggregates.FirstOrDefault(x => x.Date == date);
                var keyRate = keyRates.LastOrDefault(x => x.Date <= date);
                var moexIndexCandle = moexIndexCandles.FirstOrDefault(x => x.Date >= date);
                var rgbiIndexCandle = rgbiIndexCandles.FirstOrDefault(x => x.Date >= date);

                if (consumerPriceIndexChange is not null)
                {
                    var consumerPriceIndexChangeValue = consumerPriceIndexChange.Value - 100.0;

                    if (consumerPriceIndexChangeValue.HasValue)
                        macroParameterItem.ConsumerPriceIndexChange = Math.Round(consumerPriceIndexChangeValue.Value, 2);
                }

                if (monetaryAggregate is not null)
                {
                    macroParameterItem.M0 = monetaryAggregate.M0;
                    macroParameterItem.M1 = monetaryAggregate.M1;
                    macroParameterItem.M2 = monetaryAggregate.M2;
                    macroParameterItem.M2X = monetaryAggregate.M2X;
                }

                if (keyRate is not null)
                    macroParameterItem.KeyRate = keyRate.Value;

                if (moexIndexCandle is not null)
                    macroParameterItem.MoexIndex = moexIndexCandle.Close;

                if (rgbiIndexCandle is not null)
                    macroParameterItem.RgbiIndex = rgbiIndexCandle.Close;

                if (macroParameterItem.M2X.HasValue && macroParameterItem.M2.HasValue)
                    macroParameterItem.Currency = macroParameterItem.M2X - macroParameterItem.M2;

                if (macroParameterItem.M2.HasValue && macroParameterItem.M1.HasValue)
                    macroParameterItem.Deposits = macroParameterItem.M2 - macroParameterItem.M1;

                int count = macroParameterItems.Count;

                if (macroParameterItems.Count > 0)
                {
                    var prevMonth = macroParameterItems[count - 1];

                    macroParameterItem.MoexIndexChange = GetChange(prevMonth.MoexIndex, macroParameterItem.MoexIndex);
                    macroParameterItem.RgbiIndexChange = GetChange(prevMonth.RgbiIndex, macroParameterItem.RgbiIndex);
                    macroParameterItem.M0Change = GetChange(prevMonth.M0, macroParameterItem.M0);
                    macroParameterItem.M1Change = GetChange(prevMonth.M1, macroParameterItem.M1);
                    macroParameterItem.M2Change = GetChange(prevMonth.M2, macroParameterItem.M2);
                    macroParameterItem.M2XChange = GetChange(prevMonth.M2X, macroParameterItem.M2X);
                    macroParameterItem.CurrencyChange = GetChange(prevMonth.Currency, macroParameterItem.Currency);
                    macroParameterItem.DepositsChange = GetChange(prevMonth.Deposits, macroParameterItem.Deposits);                    
                    macroParameterItem.M1ConsumerPriceIndexDifference = GetDifference(macroParameterItem.M1Change, macroParameterItem.ConsumerPriceIndexChange);
                }

                if (macroParameterItems.Count > 11)
                {
                    var prevYear = macroParameterItems[count - 12];

                    macroParameterItem.MoexIndexYearChange = GetChange(prevYear.MoexIndex, macroParameterItem.MoexIndex);
                    macroParameterItem.RgbiIndexYearChange = GetChange(prevYear.RgbiIndex, macroParameterItem.RgbiIndex);
                    macroParameterItem.M0YearChange = GetChange(prevYear.M0, macroParameterItem.M0);
                    macroParameterItem.M1YearChange = GetChange(prevYear.M1, macroParameterItem.M1);
                    macroParameterItem.M2YearChange = GetChange(prevYear.M2, macroParameterItem.M2);
                    macroParameterItem.M2XYearChange = GetChange(prevYear.M2X, macroParameterItem.M2X);
                    macroParameterItem.CurrencyYearChange = GetChange(prevYear.Currency, macroParameterItem.Currency);
                    macroParameterItem.DepositsYearChange = GetChange(prevYear.Deposits, macroParameterItem.Deposits);
                    macroParameterItem.ConsumerPriceIndexYearChange = GetConsumerPriceIndexYearChange(macroParameterItem, macroParameterItems);
                    macroParameterItem.M1ConsumerPriceIndexYearDifference = GetDifference(macroParameterItem.M1YearChange, macroParameterItem.ConsumerPriceIndexYearChange);
                }

                macroParameterItems.Add(macroParameterItem);
            }

            var response = new GetAnalyticMacroParameterListResponse
            {
                MacroParameters = [.. macroParameterItems.OrderByDescending(x => x.Date)]
            };

            return response;
        }

        private static double GetConsumerPriceIndexYearChange(GetAnalyticMacroParameterItemListResponse macroParameterItem, List<GetAnalyticMacroParameterItemListResponse> macroParameterItems)
        {
            double result = (macroParameterItem.ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 1].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 2].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 3].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 4].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 5].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 6].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 7].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 8].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 9].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 10].ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            result *= (macroParameterItems[macroParameterItems.Count - 11].ConsumerPriceIndexChange.Value + 100.0) / 100.0;

            result =- 1;

            return Math.Round(result, 2);
        }

        private static double? GetChange(double? prevValue, double? value)
        {
            if (!prevValue.HasValue) return null;
            if (!value.HasValue) return null;

            var change = (value.Value - prevValue.Value) / prevValue.Value * 100.0;

            return Math.Round(change, 2);
        }

        private static double? GetDifference(double? firstValue, double? secondValue)
        {
            if (!firstValue.HasValue) return null;
            if (!secondValue.HasValue) return null;

            var difference = firstValue.Value - secondValue.Value;

            return Math.Round(difference, 2);
        }
    }
}
