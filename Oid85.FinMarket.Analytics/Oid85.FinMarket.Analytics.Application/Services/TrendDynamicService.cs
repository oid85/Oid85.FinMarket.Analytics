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
            var dates = DateUtils.GetDates(startDate, today);

            var response = new GetTrendDynamicResponse
            {
                Dates = dates,
                Indexes = GetTrendDynamicData(dates, startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Index)], ultimateSmootherData, candleData, dividendData, scoreData),
                Shares = GetTrendDynamicData(dates, startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Share)], ultimateSmootherData, candleData, dividendData, scoreData),
                Futures = GetTrendDynamicData(dates, startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Future)], ultimateSmootherData, candleData, dividendData, scoreData)
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
            Dictionary<string, FundamentalScore> scoreData)
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
                    DividendYield = dividendData.TryGetValue(instrument.Ticker, out Dividend? value) ? Math.Round(value.Yield.Value, 1) : null,
                    Score = scoreData.TryGetValue(instrument.Ticker, out FundamentalScore? score) ? score : null,
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
