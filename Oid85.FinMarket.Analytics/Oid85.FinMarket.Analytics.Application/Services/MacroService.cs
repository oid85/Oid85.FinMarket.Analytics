using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class MacroService(
        IStorageApiClient storageApiClient)
        : IMacroService
    {
        /// <inheritdoc />
        public async Task<CreateOrUpdateAnalyticMacroParameterResponse> CreateOrUpdateAnalyticMacroParameterAsync(CreateOrUpdateAnalyticMacroParameterRequest request)
        {
            await storageApiClient.CreateOrUpdateConsumerPriceIndexChangeAsync(
                new CreateOrUpdateConsumerPriceIndexChangeRequest { Date = request.Date, Value = request.ConsumerPriceIndexChange });

            await storageApiClient.CreateOrUpdateMonetaryAggregateAsync(
                new CreateOrUpdateMonetaryAggregateRequest { Date = request.Date, M0 = request.M0, M1 = request.M1, M2 = request.M2, M2X = request.M2X });

            await storageApiClient.CreateOrUpdateKeyRateAsync(
                new CreateOrUpdateKeyRateRequest { Date = request.Date, Value = request.KeyRate });

            return new CreateOrUpdateAnalyticMacroParameterResponse();
        }

        /// <inheritdoc />
        public async Task<GetAnalyticMacroParameterListResponse> GetAnalyticMacroParameterListAsync(GetAnalyticMacroParameterListRequest request)
        {
            var consumerPriceIndexChanges = (await storageApiClient.GetConsumerPriceIndexChangeListAsync(new())).Result.ConsumerPriceIndexChanges.OrderBy(x => x.Date).ToList();
            var monetaryAggregates = (await storageApiClient.GetMonetaryAggregateListAsync(new())).Result.MonetaryAggregates.OrderBy(x => x.Date).ToList();
            var keyRates = (await storageApiClient.GetKeyRateListAsync(new())).Result.KeyRates.OrderBy(x => x.Date).ToList();            
            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-6));
            var to = DateOnly.FromDateTime(DateTime.Today);
            var moexIndexCandles = (await storageApiClient.GetCandleListAsync(new GetCandleListRequest { Ticker = KnownIndexTickers.IMOEX, From = from, To = to })).Result.Candles.OrderBy(x => x.Date).ToList();
            var rgbiIndexCandles = (await storageApiClient.GetCandleListAsync(new GetCandleListRequest { Ticker = KnownIndexTickers.RGBI, From = from, To = to })).Result.Candles.OrderBy(x => x.Date).ToList();
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

        /// <inheritdoc />
        public async Task<GetAnalyticMacroParameterDiagramResponse> GetAnalyticMacroParameterDiagramAsync(GetAnalyticMacroParameterDiagramRequest request)
        {            
            var dates = DateUtils.GetMonthDates(
                DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                DateOnly.FromDateTime(DateTime.Today));

            var consumerPriceIndexChanges = (await storageApiClient.GetConsumerPriceIndexChangeListAsync(new())).Result.ConsumerPriceIndexChanges.OrderBy(x => x.Date).Where(x => x.Value.HasValue).ToList();
            var keyRates = (await storageApiClient.GetKeyRateListAsync(new())).Result.KeyRates.OrderBy(x => x.Date).Where(x => x.Value.HasValue).ToList();
            var vvps = (await storageApiClient.GetVvpListAsync(new())).Result.Vvps.OrderBy(x => x.Date).Where(x => x.Delta.HasValue).ToList();
            var macroParameters = (await GetAnalyticMacroParameterListAsync(new ())).MacroParameters.OrderBy(x => x.Date).Where(x => x.M1ConsumerPriceIndexYearDifference.HasValue).ToList();

            var keyRateSeries = new AnalyticMacroParameterSeries { Name = "Ставка ЦБ, %", Color = KnownColors.Blue, ColorFill = KnownColors.Blue };
            var cpiSeries = new AnalyticMacroParameterSeries { Name = "Инфляция, % гг", Color = KnownColors.Orange, ColorFill = KnownColors.Orange };
            var deltaSeries = new AnalyticMacroParameterSeries { Name = "Разность между ставкой ЦБ и инфляцией, %", Color = KnownColors.Green, ColorFill = KnownColors.Green };
            var vvpSeries = new AnalyticMacroParameterSeries { Name = "Прирост ВВП, %", Color = KnownColors.Blue, ColorFill = KnownColors.LightBlue };
            var moneyAggregatesCpiSeries = new AnalyticMacroParameterSeries { Name = "изм. M1 минус изм. инфляции, %", Color = KnownColors.Blue, ColorFill = KnownColors.LightBlue };
            var moexSeries = new AnalyticMacroParameterSeries { Name = "Индекс IMOEX", Color = KnownColors.Green, ColorFill = KnownColors.Green };

            for (int i = 12; i < dates.Count; i++)
            {
                DateOnly date = dates[i];

                var moneyAggregatesCpi = macroParameters.FindLast(x => x.Date <= date)!.M1ConsumerPriceIndexYearDifference;
                var moex = macroParameters.FindLast(x => x.Date <= date)!.MoexIndex;
                var keyRate = keyRates.FindLast(x => x.Date <= date)!.Value;
                var vvp = vvps.FindLast(x => x.Date <= date)!.Delta;
                var cpi = (consumerPriceIndexChanges.Where(x => x.Date <= date).TakeLast(12).Select(x => 1.0 + (x.Value - 100.0) / 100.0).Aggregate((x, y) => x * y) - 1.0) * 100.0;                
                                 
                var keyRateCpiDelta = keyRate!.Value - cpi!.Value;

                moneyAggregatesCpiSeries.Data.Add(new() { Date = date, Value = moneyAggregatesCpi!.Value.RoundTo(2) });
                moexSeries.Data.Add(new() { Date = date, Value = moex!.Value.RoundTo(2) });
                keyRateSeries.Data.Add(new () { Date = date, Value = keyRate!.Value.RoundTo(2) });
                cpiSeries.Data.Add(new () { Date = date, Value = cpi!.Value.RoundTo(2) });
                deltaSeries.Data.Add(new() { Date = date, Value = keyRateCpiDelta.RoundTo(2) });
                vvpSeries.Data.Add(new() { Date = date, Value = vvp.RoundTo(2) });
            }

            return new () 
            {
                KeyRateSeries = [keyRateSeries, cpiSeries, deltaSeries],
                VvpSeries = [vvpSeries],
                MoneyAggregatesCpiSeries = [moneyAggregatesCpiSeries],
                MoexSeries = [moexSeries]
            };
        }

        private static double GetConsumerPriceIndexYearChange(GetAnalyticMacroParameterItemListResponse macroParameterItem, List<GetAnalyticMacroParameterItemListResponse> macroParameterItems)
        {
            double result = 1.0;

            if (macroParameterItem.ConsumerPriceIndexChange.HasValue) result = (macroParameterItem.ConsumerPriceIndexChange.Value + 100.0) / 100.0;
            if (macroParameterItems[^1].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^1].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^2].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^2].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^3].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^3].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^4].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^4].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^5].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^5].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^6].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^6].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^7].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^7].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^8].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^8].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^9].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^9].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^10].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^10].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;
            if (macroParameterItems[^11].ConsumerPriceIndexChange.HasValue) result *= (macroParameterItems[^11].ConsumerPriceIndexChange!.Value + 100.0) / 100.0;

            return Math.Round((result - 1.0) * 100.0, 2);
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
