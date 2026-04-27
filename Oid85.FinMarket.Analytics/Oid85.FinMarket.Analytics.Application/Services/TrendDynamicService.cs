using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class TrendDynamicService(
        IInstrumentRepository instrumentRepository,
        IDataService dataService)
        : ITrendDynamicService
    {
        /// <inheritdoc />
        public async Task<GetTrendDynamicResponse> GetTrendDynamicAsync(GetTrendDynamicRequest request)
        {
            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * request.LastDaysCount));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var instruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Where(x => x.IsSelected).ToList();
            var tickers = instruments!.Select(x => x.Ticker).ToList();
            var candleData = await dataService.GetCandleDataAsync(tickers);
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);
            var dividendData = await dataService.GetDividendDataAsync(tickers);
            var scoreData = await dataService.GetFundamentalScoreDataAsync(tickers);
            var forecastData = await dataService.GetConsensusForecastDataAsync();
            var nataliaBaffetovnaForecastData = await dataService.GetNataliaBaffetovnaForecastDataAsync(tickers);            
            var financeMarkerForecastData = await dataService.GetFinanceMarkerForecastDataAsync(tickers);
            var vladProDengiForecastData = await dataService.GetVladProDengiForecastDataAsync(tickers);
            var mozgovikForecastData = await dataService.GetMozgovikForecastDataAsync(tickers);
            var predictNetProfitForecastData = await dataService.GetPredictNetProfitForecastDataAsync(tickers);
            var fillFundamentalData = await dataService.GetFillFundamentalDataAsync(tickers);
            var extData = await dataService.GetExtDataAsync(tickers);
            var dates = DateUtils.GetDates(startDate, today);

            var response = new GetTrendDynamicResponse
            {
                Dates = dates,
                Indexes = GetTrendDynamicData(dates, startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Index)], ultimateSmootherData, candleData, dividendData, scoreData, forecastData, nataliaBaffetovnaForecastData, financeMarkerForecastData, vladProDengiForecastData, mozgovikForecastData, predictNetProfitForecastData, fillFundamentalData, extData),
                Shares = GetTrendDynamicData(dates, startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Share)], ultimateSmootherData, candleData, dividendData, scoreData, forecastData, nataliaBaffetovnaForecastData, financeMarkerForecastData, vladProDengiForecastData, mozgovikForecastData, predictNetProfitForecastData, fillFundamentalData, extData),
                Futures = GetTrendDynamicData(dates, startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Future)], ultimateSmootherData, candleData, dividendData, scoreData, forecastData, nataliaBaffetovnaForecastData, financeMarkerForecastData, vladProDengiForecastData, mozgovikForecastData, predictNetProfitForecastData, fillFundamentalData, extData)
            };

            return response;
        }

        private static List<TrendDynamicData> GetTrendDynamicData(
            List<DateOnly> dates,
            DateOnly from,
            DateOnly to,
            List<Instrument> instruments,
            Dictionary<string, List<DateValue<double>>> ultimateSmootherData,
            Dictionary<string, List<Candle>> candleData,
            Dictionary<string, Dividend> dividendData,
            Dictionary<string, FundamentalScore> scoreData,
            Dictionary<string, Forecast> forecastData,
            Dictionary<string, Forecast> nataliaBaffetovnaForecastData,
            Dictionary<string, Forecast> financeMarkerForecastData,
            Dictionary<string, Forecast> vladProDengiForecastData,
            Dictionary<string, Forecast> mozgovikForecastData,
            Dictionary<string, Forecast> predictNetProfitForecastData,
            Dictionary<string, bool> fillFundamentalData,
            Dictionary<string, (string? DividendPolyticInfo, string? GrowthDriverInfo, string? RiskInfo, string? Concept)> extData
            )
        {
            var data = new List<TrendDynamicData>();

            foreach (var instrument in instruments)
            {
                if (!candleData.ContainsKey(instrument.Ticker)) continue;
                if (!ultimateSmootherData.ContainsKey(instrument.Ticker)) continue;

                var trendDynamicData = new TrendDynamicData()
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    InPortfolio = instrument.InPortfolio,
                    DividendYield = dividendData.TryGetValue(instrument.Ticker, out Dividend? value) ? Math.Round(value.Yield!.Value, 1) : null,
                    Score = scoreData.TryGetValue(instrument.Ticker, out FundamentalScore? score) ? score : null,
                    Forecast = forecastData.TryGetValue(instrument.Ticker, out Forecast? forecast) ? forecast : null,
                    NataliaBaffetovnaForecast = nataliaBaffetovnaForecastData.TryGetValue(instrument.Ticker, out Forecast? nataliaBaffetovnaForecast) ? nataliaBaffetovnaForecast : null,
                    FinanceMarkerForecast = financeMarkerForecastData.TryGetValue(instrument.Ticker, out Forecast? financeMarkerForecast) ? financeMarkerForecast : null,
                    VladProDengiForecast = vladProDengiForecastData.TryGetValue(instrument.Ticker, out Forecast? vladProDengiForecast) ? vladProDengiForecast : null,
                    MozgovikForecast = mozgovikForecastData.TryGetValue(instrument.Ticker, out Forecast? mozgovikForecast) ? mozgovikForecast : null,
                    PredictNetProfitForecast = predictNetProfitForecastData.TryGetValue(instrument.Ticker, out Forecast? predictNetProfitForecast) ? predictNetProfitForecast : null,
                    FillData = fillFundamentalData.TryGetValue(instrument.Ticker, out bool fillFundamental) ? fillFundamental : false,
                    Concept = (extData.TryGetValue(instrument.Ticker, out var extDataItem) ? extDataItem.Concept : null) ?? "...",
                    Items = []
                };

                var ultimateSmootherValues = ultimateSmootherData[instrument.Ticker].Where(x => x.Date >= from && x.Date <= to).ToList();
                var candles = candleData[instrument.Ticker].Where(x => x.Date >= from && x.Date <= to).ToList();

                var dictionary = dates.ToDictionary(key => key, value => new TrendDynamicDataItem() { Date = value, Trend = null, Delta = null, Price = null });

                for (int i = 1; i < candles.Count; i++)
                {
                    var date = candles[i].Date;
                    dictionary[date].Trend = ultimateSmootherValues[i].Value > ultimateSmootherValues[i - 1].Value ? 1 : -1;
                    dictionary[date].Delta = Math.Round((candles[i].Close - candles[i - 1].Close) / candles[i - 1].Close * 100.0, 1);
                    dictionary[date].Price = Math.Round(candles[i].Close, 4);
                }

                trendDynamicData.Items = [.. dictionary.Values];

                data.Add(trendDynamicData);
            }

            var inPortfolioItems = data
                .Where(x => x.InPortfolio)
                .OrderByDescending(x =>
                {
                    var reverse = x.Items.Select(x => x.Trend).Where(x => x != null).AsEnumerable().Reverse();
                    var count = reverse.TakeWhile(x => x == 1).Count();
                    return count;
                })
                .ToList();

            var notInPortfolioItems = data
                .Where(x => !x.InPortfolio)
                .OrderByDescending(x =>
                {
                    var reverse = x.Items.Select(x => x.Trend).Where(x => x != null).AsEnumerable().Reverse();
                    var count = reverse.TakeWhile(x => x == 1).Count();
                    return count;
                })
                .ToList();

            return [.. inPortfolioItems, .. notInPortfolioItems];
        }
    }
}
