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
        IParameterRepository parameterRepository,
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
            var etfs = instruments.Where(x => x.Type == KnownInstrumentTypes.Etf).ToList();
            var tickers = instruments!.Select(x => x.Ticker).ToList();
            var candleData = await dataService.GetCandleDataAsync(tickers);
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);
            var weeks = DateUtils.GetWeeks(startDate, today);
            bool showInPortfolio = (await parameterRepository.GetParameterValueAsync(KnownParameters.ShowInPortfolio)) == "true";

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
                Futures = GetWeekDeltaDataList(futures),
                Etfs = GetWeekDeltaDataList(etfs)
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

                    weekDeltaDataList.Add(
                        new()
                        {
                            Ticker = instrument.Ticker,
                            Name = instrument.Name,
                            InPortfolio = instrument.InPortfolio && showInPortfolio,
                            Items = weekDeltaData
                        });
                }

                var inPortfolioItems = weekDeltaDataList
                    .Where(x => x.InPortfolio)
                    .OrderByDescending(x =>
                    {
                        var reverse = x.Items.Select(x => x.Delta).Where(x => x != null && x != 0.0).AsEnumerable().Reverse();
                        var count = reverse.TakeWhile(x => x > 0).Count();
                        return count;
                    })
                    .ToList();

                var notInPortfolioItems = weekDeltaDataList
                    .Where(x => !x.InPortfolio)
                    .OrderByDescending(x =>
                    {
                        var reverse = x.Items.Select(x => x.Delta).Where(x => x != null && x != 0.0).AsEnumerable().Reverse();
                        var count = reverse.TakeWhile(x => x > 0).Count();
                        return count;
                    })
                    .ToList();

                return [.. inPortfolioItems, .. notInPortfolioItems];
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

                return new WeekDeltaDataItem { Delta = delta.RoundTo(2), Price = lastPrice.RoundTo(4) };
            }
        }
    }
}
