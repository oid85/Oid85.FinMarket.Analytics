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
            var moexIndexCandles = (await finMarketStorageServiceApiClient.GetCandleListAsync(new GetCandleListRequest { Ticker = KnownIndexTickers.IMOEX, From = from, To = to })).Result.Candles;
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

                if (macroParameterItem.M2X.HasValue && macroParameterItem.M2.HasValue)
                    macroParameterItem.Currency = macroParameterItem.M2X - macroParameterItem.M2;

                if (macroParameterItem.M2.HasValue && macroParameterItem.M1.HasValue)
                    macroParameterItem.Deposits = macroParameterItem.M2 - macroParameterItem.M1;

                if (macroParameterItems.Count > 0)
                {
                    var lastMacroParameterItem = macroParameterItems.Last();

                    macroParameterItem.MoexIndexChange = GetChange(lastMacroParameterItem.MoexIndex, macroParameterItem.MoexIndex);
                    macroParameterItem.M0Change = GetChange(lastMacroParameterItem.M0, macroParameterItem.M0);
                    macroParameterItem.M1Change = GetChange(lastMacroParameterItem.M1, macroParameterItem.M1);
                    macroParameterItem.M2Change = GetChange(lastMacroParameterItem.M2, macroParameterItem.M2);
                    macroParameterItem.M2XChange = GetChange(lastMacroParameterItem.M2X, macroParameterItem.M2X);
                    macroParameterItem.CurrencyChange = GetChange(lastMacroParameterItem.Currency, macroParameterItem.Currency);
                    macroParameterItem.DepositsChange = GetChange(lastMacroParameterItem.Deposits, macroParameterItem.Deposits);

                    if (macroParameterItem.M1Change.HasValue && macroParameterItem.ConsumerPriceIndexChange.HasValue)
                        if (macroParameterItem.M1Change.HasValue && macroParameterItem.ConsumerPriceIndexChange.HasValue)
                        {
                            var m1ConsumerPriceIndexDifferenceChangeValue = macroParameterItem.M1Change.Value - macroParameterItem.ConsumerPriceIndexChange.Value;
                            macroParameterItem.M1ConsumerPriceIndexDifferenceChange = Math.Round(m1ConsumerPriceIndexDifferenceChangeValue, 2);
                        }
                }

                macroParameterItems.Add(macroParameterItem);
            }

            var response = new GetAnalyticMacroParameterListResponse
            {
                MacroParameters = [.. macroParameterItems.OrderByDescending(x => x.Date)]
            };

            return response;
        }

        private static double? GetChange(double? prevValue, double? value)
        {
            if (!prevValue.HasValue) return null;
            if (!value.HasValue) return null;

            var change = (value.Value - prevValue.Value) / prevValue.Value * 100.0;

            return Math.Round(change, 2);
        }
    }
}
