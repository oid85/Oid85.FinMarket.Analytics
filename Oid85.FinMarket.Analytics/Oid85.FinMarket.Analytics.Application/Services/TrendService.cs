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
    public class TrendService(
        IInstrumentRepository instrumentRepository,
        IDataService dataService) 
        : ITrendService
    {
        /// <inheritdoc />
        public async Task<GetTrendDynamicResponse> GetTrendDynamicAsync(GetTrendDynamicRequest request)
        {
            var monthAgo = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var instruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Where(x => x.IsSelected).ToList();
            var tickers = instruments!.Select(x => x.Ticker).ToList();
            var candleData = await dataService.GetCandleDataAsync(tickers);
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);
            var dates = DateUtils.GetDates(monthAgo, today);

            var response = new GetTrendDynamicResponse();
            
            response.Dates = dates;

            response.Indexes = GetTrendDynamicData(dates,monthAgo, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Index)], ultimateSmootherData, candleData);
            response.Shares = GetTrendDynamicData(dates, monthAgo, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Share)], ultimateSmootherData, candleData);
            response.Futures = GetTrendDynamicData(dates, monthAgo, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Future)], ultimateSmootherData, candleData);
            response.Bonds = GetTrendDynamicData(dates, monthAgo, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Bond)], ultimateSmootherData, candleData);

            return response;
        }

        private static List<TrendDynamicData> GetTrendDynamicData(
            List<DateOnly> dates,
            DateOnly from, 
            DateOnly to, 
            List<Instrument> instruments, 
            Dictionary<string, List<DateValue<double>>> ultimateSmootherData,
            Dictionary<string, List<Candle>> candleData)
        {
            var data = new List<TrendDynamicData>();

            foreach (var instrument in instruments)
            {
                var trendDynamicData = new TrendDynamicData() { Ticker = instrument.Ticker, Name = instrument.Name, Items = [] };
                var ultimateSmootherValues = ultimateSmootherData[instrument.Ticker].Where(x => x.Date >= from && x.Date <= to).ToList();
                var candles = candleData[instrument.Ticker].Where(x => x.Date >= from && x.Date <= to).ToList();

                var dictionary = dates.ToDictionary(key => key, value => new TrendDynamicDataItem() { Date = value, Trend = null, Delta = null, Price = null });

                for (int i = 1; i < candles.Count; i++)
                {
                    var date = candles[i].Date;
                    dictionary[date].Trend = ultimateSmootherValues[i].Value > ultimateSmootherValues[i - 1].Value ? 1 : -1;
                    dictionary[date].Delta = Math.Round((candles[i].Close - candles[i - 1].Close) / candles[i - 1].Close * 100.0, 2);
                    dictionary[date].Price = candles[i].Close;
                }

                trendDynamicData.Items = [.. dictionary.Values];

                data.Add(trendDynamicData);
            }

            var result = data.OrderByDescending(x =>
            {
                var reverse = x.Items.Select(x => x.Trend).Where(x => x != null).AsEnumerable().Reverse();
                var count = reverse.TakeWhile(x => x == 1).Count();
                return count;
            }).ToList();

            return result;
        }

        /// <inheritdoc />
        public async Task<GetCompareTrendResponse> GetCompareTrendAsync(GetCompareTrendRequest request)
        {
            var monthAgo = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var instruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Where(x => x.IsSelected).ToList();
            var tickers = instruments!.Select(x => x.Ticker).ToList();
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);
            var dates = DateUtils.GetDates(monthAgo, today);

            var response = new GetCompareTrendResponse();

            foreach (var item in ultimateSmootherData)
                response.Series.Add(
                    new GetCompareTrendSeriesResponse
                    {
                        Name = item.Key,
                        Data = GetSeriesData(dates, item.Value)
                    });

            return response;
        }

        private static List<GetCompareTrendSeriesItemResponse> GetSeriesData(List<DateOnly> dates, List<DateValue<double>> dateValues)
        {
            var series = new List<GetCompareTrendSeriesItemResponse>();

            foreach (var date in dates)
                series.Add(
                    new GetCompareTrendSeriesItemResponse
                    {
                        Date = date,
                        Value = dateValues.Find(x => x.Date == date)?.Value ?? null
                    });

            return series;
        }
    }
}
