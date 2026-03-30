using Oid85.FinMarket.Analytics.Application.Helpers;
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
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);
            var weeks = DateUtils.GetWeeks(startDate, today);

            var response = new GetWeekDeltaResponse
            {
                Weeks = [.. weeks.Select(x =>
                new WeekDeltaHeaderItem
                {
                    WeekNumber = x.WeekNumber,
                    WeekStartDay = x.WeekStartDay,
                    WeekEndDay = x.WeekEndDay
                })],
                Shares = GetWeekDeltaDataList(shares),
                Indexes = GetWeekDeltaDataList(indexes),
                Futures = GetWeekDeltaDataList(futures)
            };

            return response;

            List<WeekDeltaData> GetWeekDeltaDataList(List<Instrument> instruments)
            {
                var weekDeltaDataList = new List<WeekDeltaData>();

                foreach (var instrument in instruments)
                {
                    if (!candleData.ContainsKey(instrument.Ticker)) continue;
                    if (!ultimateSmootherData.ContainsKey(instrument.Ticker)) continue;

                    List<WeekDeltaDataItem> weekDeltaData = [.. weeks.Select(x => GetWeekDeltaDataItem(instrument.Ticker, x.WeekStartDay, x.WeekEndDay))];

                    var candles = candleData[instrument.Ticker].Where(x => x.Date >= startDate).ToList();
                    var ultimateSmoothers = ultimateSmootherData[instrument.Ticker].Where(x => x.Date >= startDate).ToList();
                    double maxPrice = candles.Select(x => x.Close).Max();
                    double lastCandlePrice = candles.Last().Close;
                    double fallingFromMax = -1 * (maxPrice - lastCandlePrice) / maxPrice * 100.0;

                    weekDeltaDataList.Add(
                        new()
                        {
                            Ticker = instrument.Ticker,
                            Name = instrument.Name,
                            InPortfolio = instrument.InPortfolio,
                            Items = weekDeltaData,
                            TrendState = TrendStateHelper.GetTrendState(ultimateSmoothers).Message,
                            FallingFromMax = Math.Round(fallingFromMax, 2)
                        });
                }

                return
                [
                    .. weekDeltaDataList.Where(x => x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.UpTrend),
                    .. weekDeltaDataList.Where(x => x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.NoTrend),
                    .. weekDeltaDataList.Where(x => x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.DownTrend),
                    .. weekDeltaDataList.Where(x => !x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.UpTrend),
                    .. weekDeltaDataList.Where(x => !x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.NoTrend),
                    .. weekDeltaDataList.Where(x => !x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.DownTrend)
                ];
            }

            WeekDeltaDataItem GetWeekDeltaDataItem(string ticker, DateOnly weekStartDay, DateOnly weekEndDay)
            {
                if (!candleData.ContainsKey(ticker))
                    return new WeekDeltaDataItem { Delta = null };

                var candles = candleData[ticker]?.Where(x => x.Date >= weekStartDay && x.Date <= weekEndDay)?.ToList();

                if (candles is null)
                    return new WeekDeltaDataItem { Delta = null };

                if (candles.Count == 0)
                    return new WeekDeltaDataItem { Delta = null };

                double firstPrice = candles.First().Close;
                double lastPrice = candles.Last().Close;

                double delta = (lastPrice - firstPrice) / firstPrice * 100.0;

                return new WeekDeltaDataItem { Delta = Math.Round(delta, 2), Price = Math.Round(lastPrice, 4) };
            }
        }
    }
}
