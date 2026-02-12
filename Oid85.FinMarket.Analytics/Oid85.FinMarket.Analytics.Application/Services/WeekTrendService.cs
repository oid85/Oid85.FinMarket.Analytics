using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class WeekTrendService(
        IInstrumentService instrumentService,
        IDataService dataService)
        : IWeekTrendService
    {
        /// <inheritdoc />
        public async Task<GetWeekDeltaResponse> GetWeekDeltaAsync(GetWeekDeltaRequest request)
        {
            var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-7 * (request.LastWeeksCount + 1)));
            var today = DateOnly.FromDateTime(DateTime.Today);

            var instruments = ((await instrumentService.GetStorageInstrumentAsync()) ?? []).ToList();
            var indexes = instruments.Where(x => x.Type == KnownInstrumentTypes.Index).ToList();
            var shares = instruments.Where(x => x.Type == KnownInstrumentTypes.Share).ToList();
            var tickers = instruments!.Select(x => x.Ticker).ToList();
            var candleData = await dataService.GetCandleDataAsync(tickers);
            var weeks = DateUtils.GetWeeks(startDate, today);

            var response = new GetWeekDeltaResponse
            {
                Weeks = [.. weeks.Select(x =>
                new WeekDeltaHeaderItem
                {
                    WeekNumber = x.WeekNumber,
                    WeekStartDay = x.WeekStartDay,
                    WeekEndDay = x.WeekEndDay
                })]
            };

            var sharesData = new List<WeekDeltaData>();

            foreach (var share in shares)
            {
                var dataItem = new WeekDeltaData
                {
                    Ticker = share.Ticker,
                    Name = share.Name,
                    Items = [.. weeks.Select(x => GetWeekDeltaDataItem(share.Ticker, x.WeekStartDay, x.WeekEndDay))]
                };

                sharesData.Add(dataItem);
            }

            response.Shares = [.. sharesData.OrderBy(x => x.Ticker)];

            var indexesData = new List<WeekDeltaData>();

            foreach (var index in indexes)
            {
                var dataItem = new WeekDeltaData
                {
                    Ticker = index.Ticker,
                    Name = index.Name,
                    Items = [.. weeks.Select(x => GetWeekDeltaDataItem(index.Ticker, x.WeekStartDay, x.WeekEndDay))]
                };

                indexesData.Add(dataItem);
            }

            response.Indexes = [.. indexesData.OrderBy(x => x.Ticker)];

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

                return new WeekDeltaDataItem() { Delta = delta, Price = lastPrice };
            }
        }        
    }
}
