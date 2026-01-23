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
            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * request.LastDaysCount));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var instruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Where(x => x.IsSelected).ToList();
            var tickers = instruments!.Select(x => x.Ticker).ToList();
            var candleData = await dataService.GetCandleDataAsync(tickers);
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);
            var dates = DateUtils.GetDates(startDate, today);

            var response = new GetTrendDynamicResponse();
            
            response.Dates = dates;

            response.Indexes = GetTrendDynamicData(dates,startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Index)], ultimateSmootherData, candleData);
            response.Shares = GetTrendDynamicData(dates, startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Share)], ultimateSmootherData, candleData);
            response.Futures = GetTrendDynamicData(dates, startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Future)], ultimateSmootherData, candleData);
            response.Bonds = GetTrendDynamicData(dates, startDate, today, [.. instruments!.Where(x => x.Type == KnownInstrumentTypes.Bond)], ultimateSmootherData, candleData);

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
            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1 * request.LastDaysCount));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var instruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Where(x => x.IsSelected).ToList();
            var tickers = instruments!.Select(x => x.Ticker).ToList();
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);
            var dates = DateUtils.GetDates(startDate, today);

            var series = new List<GetCompareTrendSeriesResponse>();

            var benchmark = ultimateSmootherData["MCFTR"].Last().Value / ultimateSmootherData["MCFTR"].First().Value;

            foreach (var pair in ultimateSmootherData)
            {
                var seriesItem = new GetCompareTrendSeriesResponse();

                seriesItem.Name = pair.Key;
                seriesItem.Data = GetNormDataValues(GetSeriesData(dates, pair.Value));
                seriesItem.Color = GetColor(seriesItem.Name, seriesItem.Data, benchmark);

                series.Add(seriesItem);
            }

            return new GetCompareTrendResponse() { Series = series };

            static string GetColor(string name, List<GetCompareTrendSeriesItemResponse> data, double benchmark)
            {
                if (data.Count == 0)
                    return "#191970";

                if (name == "MCFTR")
                    return "#191970";

                if (data.Last().Value > benchmark)
                    return "#00CC66";

                if (data.Last().Value < benchmark)
                    return "#FF6633";

                return "#191970";
            }
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

        private static List<GetCompareTrendSeriesItemResponse> GetNormDataValues(List<GetCompareTrendSeriesItemResponse> items)
        {
            if (items.Count == 0)
                return [];

            if (!items.Any(x => x.Value is not null))
                return items;

            var divider = items.First(x => x.Value is not null).Value;

            var result = new List<GetCompareTrendSeriesItemResponse>();

            foreach (var dateValue in items)
                result.Add(
                    new GetCompareTrendSeriesItemResponse
                    {
                        Date = dateValue.Date,
                        Value = dateValue.Value is null ? null : dateValue.Value / divider
                    });

            return result;
        }
    }
}
