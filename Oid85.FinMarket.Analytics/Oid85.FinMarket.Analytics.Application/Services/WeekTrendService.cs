using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class WeekTrendService(
        IInstrumentRepository instrumentRepository,
        IDataService dataService)
        : IWeekTrendService
    {
        /// <inheritdoc />
        public async Task<GetWeekDeltaResponse> GetWeekDeltaAsync(GetWeekDeltaRequest request)
        {
            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7 * (request.LastWeeksCount + 1)));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var instruments = ((await instrumentRepository.GetInstrumentsAsync()) ?? []).Where(x => x.IsSelected).ToList();
            var tickers = instruments!.Select(x => x.Ticker).ToList();
            var candleData = await dataService.GetCandleDataAsync(tickers);
            var weeks = DateUtils.GetWeeks(startDate, today);

            var response = new GetWeekDeltaResponse
            {
                Headers = [.. weeks.Select(x =>
                new WeekDeltaHeaderItem
                {
                    WeekNumber = x.WeekNumber,
                    WeekStartDay = x.WeekStartDay,
                    WeekEndDay = x.WeekEndDay
                })]
            };

            var data = new List<WeekDeltaData>();

            foreach (var instrument in instruments)
            {
                var dataItem = new WeekDeltaData
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    Items = [.. weeks.Select(x => GetWeekDeltaDataItem(instrument.Ticker, x.WeekStartDay, x.WeekEndDay))]
                };

                data.Add(dataItem);
            }

            response.Data = [.. data.OrderBy(x => x.Ticker)];

            return response;

            WeekDeltaDataItem GetWeekDeltaDataItem(string ticker, DateOnly weekStartDay, DateOnly weekEndDay)
            {
                var candles = candleData[ticker]?.Where(x => x.Date >= weekStartDay && x.Date <= weekEndDay)?.ToList();

                if (candles is null)
                    return new WeekDeltaDataItem() { Delta = null };

                if (candles.Count == 0)
                    return new WeekDeltaDataItem() { Delta = null };

                double firstPrice = candles.First().Close;
                double lastPrice = candles.Last().Close;

                double delta = Math.Round((lastPrice - firstPrice) / firstPrice * 100.0, 2);

                return new WeekDeltaDataItem() { Delta = delta };
            }
        }        
    }
}
