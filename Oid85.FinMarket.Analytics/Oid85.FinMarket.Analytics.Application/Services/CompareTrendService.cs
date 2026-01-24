using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class CompareTrendService(
        IInstrumentRepository instrumentRepository,
        IDataService dataService)
        : ICompareTrendService
    {
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

            var benchmarkValues = ultimateSmootherData["MCFTR"].Where(x => x.Date >= startDate && x.Date <= today).ToList();
            var benchmarkChange = benchmarkValues.Last().Value / benchmarkValues.First().Value;

            foreach (var pair in ultimateSmootherData)
            {
                var seriesItem = new GetCompareTrendSeriesResponse();

                seriesItem.Name = pair.Key;
                seriesItem.Data = GetNormDataValues(GetSeriesData(dates, pair.Value));
                seriesItem.Color = GetColor(seriesItem.Name, seriesItem.Data, benchmarkChange);

                series.Add(seriesItem);
            }

            return new GetCompareTrendResponse() { Series = series };

            static string GetColor(string name, List<GetCompareTrendSeriesItemResponse> data, double benchmarkChange)
            {
                if (data.Count == 0)
                    return "#191970";

                if (name == "MCFTR")
                    return "#191970";

                if (data.Last(x => x.Value is not null).Value > benchmarkChange)
                    return "#00CC66";

                if (data.Last(x => x.Value is not null).Value < benchmarkChange)
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
