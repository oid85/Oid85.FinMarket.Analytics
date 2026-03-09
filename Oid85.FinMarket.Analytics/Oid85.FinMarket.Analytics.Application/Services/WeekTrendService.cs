using Oid85.FinMarket.Analytics.Application.Helpers;
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
            var indexes = instruments.Where(x => x.Type == KnownInstrumentTypes.Index).ToList();
            var shares = instruments.Where(x => x.Type == KnownInstrumentTypes.Share).ToList();
            var futures = instruments.Where(x => x.Type == KnownInstrumentTypes.Future).ToList();
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
                List<WeekDeltaDataItem> weekDeltaData = [.. weeks.Select(x => GetWeekDeltaDataItem(share.Ticker, x.WeekStartDay, x.WeekEndDay))];

                var candles = candleData[share.Ticker].Where(x => x.Date >= startDate).ToList();
                double maxPrice = candles.Select(x => x.Close).Max();
                double lastCandlePrice = candles.Last().Close;
                double fallingFromMax = -1 * (maxPrice - lastCandlePrice) / maxPrice * 100.0;

                var dataItem = new WeekDeltaData
                {
                    Ticker = share.Ticker,
                    Name = share.Name,
                    InPortfolio = share.InPortfolio,                    
                    Items = weekDeltaData,
                    TrendState = TrendStateHelper.GetTrendState(weekDeltaData).Message,
                    FallingFromMax = Math.Round(fallingFromMax, 2)
                };

                sharesData.Add(dataItem);
            }

            response.Shares = [.. sharesData.OrderBy(x => x.Ticker)];

            var indexesData = new List<WeekDeltaData>();

            foreach (var index in indexes)
            {
                List<WeekDeltaDataItem> weekDeltaData = [.. weeks.Select(x => GetWeekDeltaDataItem(index.Ticker, x.WeekStartDay, x.WeekEndDay))];

                var candles = candleData[index.Ticker].Where(x => x.Date >= startDate).ToList();
                double maxPrice = candles.Select(x => x.Close).Max();
                double lastCandlePrice = candles.Last().Close;
                double fallingFromMax = -1 * (maxPrice - lastCandlePrice) / maxPrice * 100.0;

                var dataItem = new WeekDeltaData
                {
                    Ticker = index.Ticker,
                    Name = index.Name,
                    InPortfolio = index.InPortfolio,
                    Items = weekDeltaData,
                    TrendState = TrendStateHelper.GetTrendState(weekDeltaData).Message,
                    FallingFromMax = Math.Round(fallingFromMax, 2)
                };

                indexesData.Add(dataItem);
            }

            response.Indexes = [.. indexesData.OrderBy(x => x.Ticker)];

            var futuresData = new List<WeekDeltaData>();

            foreach (var future in futures)
            {
                List<WeekDeltaDataItem> weekDeltaData = [.. weeks.Select(x => GetWeekDeltaDataItem(future.Ticker, x.WeekStartDay, x.WeekEndDay))];

                var candles = candleData[future.Ticker].Where(x => x.Date >= startDate).ToList();
                double maxPrice = candles.Select(x => x.Close).Max();
                double lastCandlePrice = candles.Last().Close;
                double fallingFromMax = -1 * (maxPrice - lastCandlePrice) / maxPrice * 100.0;

                var dataItem = new WeekDeltaData
                {
                    Ticker = future.Ticker,
                    Name = future.Name,
                    InPortfolio = future.InPortfolio,
                    Items = weekDeltaData,
                    TrendState = TrendStateHelper.GetTrendState(weekDeltaData).Message,
                    FallingFromMax = Math.Round(fallingFromMax, 2)
                };

                futuresData.Add(dataItem);
            }

            response.Futures = [.. futuresData.OrderBy(x => x.Ticker)];

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

                double delta = (lastPrice - firstPrice) / firstPrice * 100.0;

                return new WeekDeltaDataItem() { Delta = Math.Round(delta, 2), Price = Math.Round(lastPrice, 4) };
            }
        }
    }
}
