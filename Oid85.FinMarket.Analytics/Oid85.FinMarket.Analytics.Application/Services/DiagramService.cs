using Oid85.FinMarket.Analytics.Application.Helpers;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Enums;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class DiagramService(
        IInstrumentService instrumentService,
        IDataService dataService,
        IParameterRepository parameterRepository)
        : IDiagramService
    {
        public async Task<GetClosePriceDiagramResponse> GetClosePriceDiagramAsync(GetClosePriceDiagramRequest request)
        {
            var allInstruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments.ToList();

            List<string> indexTickers = [KnownIndexTickers.IMOEX, KnownIndexTickers.MCFTR, KnownIndexTickers.RGBI, KnownIndexTickers.RVI];

            var indexes = allInstruments
                .Where(x => x.Type == KnownInstrumentTypes.Index)
                .OrderBy(x => indexTickers.Contains(x.Ticker))
                .ToList();

            var instrumentsInPortfolio = allInstruments
                .Where(x => x.Type == KnownInstrumentTypes.Share || x.Type == KnownInstrumentTypes.Etf)
                .Where(x => x.IsSelected)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var instrumentsNotInPortfolio = allInstruments
                .Where(x => x.Type == KnownInstrumentTypes.Share || x.Type == KnownInstrumentTypes.Etf)
                .Where(x => x.IsSelected)
                .Where(x => !x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            List<GetAnalyticInstrumentListItemResponse> instruments = [.. indexes, .. instrumentsInPortfolio, .. instrumentsNotInPortfolio];

            var tickers = instruments.Select(x => x.Ticker).ToList();

            var response = new GetClosePriceDiagramResponse();

            var closePriceDiagramData = await dataService.GetClosePriceDataAsync(tickers);
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);
            var dividendData = await dataService.GetDividendDataAsync(tickers);

            var items = new List<GetClosePriceDiagramItemResponse>();

            var years = int.Parse((await parameterRepository.GetParameterValueAsync(KnownParameters.DiagramPeriodShortYears)) ?? "1");

            foreach (var instrument in instruments)
            {
                items.Add(
                    new()
                    {
                        Ticker = instrument.Ticker,
                        Name = instrument.Name,
                        InPortfolio = instrument.InPortfolio,
                        Data = [.. ultimateSmootherData[instrument.Ticker].Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddYears(-1 * years))).Select(x => new GetClosePriceDiagramDateValueResponse { Date = x.Date, Value = x.Value })],
                        TrendState = TrendStateHelper.GetTrendState(ultimateSmootherData[instrument.Ticker]).Message,
                        DividendYield = dividendData.TryGetValue(instrument.Ticker, out Dividend? value) ? value.Yield!.Value.RoundTo(1) : null
                    });
            }

            response.Items =
                [
                    .. items.Where(x => x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.UpTrend),
                    .. items.Where(x => x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.NoTrend),
                    .. items.Where(x => x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.DownTrend),
                    .. items.Where(x => !x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.UpTrend),
                    .. items.Where(x => !x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.NoTrend),
                    .. items.Where(x => !x.InPortfolio).Where(x => x.TrendState == KnownTrendStates.DownTrend)
                ];

            return response;
        }

        public async Task<GetTrendAggregateDiagramResponse> GetTrendAggregateDiagramAsync(GetTrendAggregateDiagramRequest request)
        {
            var instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments                
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => x.InPortfolio)
                .ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();

            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);

            var from = DateOnly.FromDateTime(DateTime.Today.AddMonths(-3));
            var to = DateOnly.FromDateTime(DateTime.Today);
            var dates = DateUtils.GetDates(from, to);

            var response = new GetTrendAggregateDiagramResponse();
            
            var trendAggregateSeries = new TrendAggregateSeries()
            {
                Name = $"Акции с трендом вверх",
                Color = KnownColors.Black,
                ColorFill = KnownColors.Green,
                Data = []
            };

            foreach (var date in dates)
            {
                int countTrendUp = 0;

                foreach (var ticker in tickers)
                {
                    var trendState = GetTrendStateByDate(date, ticker);

                    if (trendState == TrendState.UpTrend)
                        countTrendUp++;
                }

                trendAggregateSeries.Data.Add(
                    new ()
                    {
                        Date = date,
                        Value = (Convert.ToDouble(countTrendUp) / Convert.ToDouble(tickers.Count) * 100.0).RoundTo(2)
                    });
            }

            response.Series = [ trendAggregateSeries ];

            return response;

            TrendState GetTrendStateByDate(DateOnly date, string ticker)
            {
                if (!ultimateSmootherData.TryGetValue(ticker, out var ultimateSmoother)) return TrendState.NoTrend;
                var ultimateSmootherFiltered = ultimateSmoother.Where(x => x.Date <= date).OrderBy(x => x.Date).TakeLast(5).ToList();
                if (ultimateSmootherFiltered.Count < 5)  return TrendState.NoTrend;
                var trendState = TrendStateHelper.GetTrendState(ultimateSmootherFiltered);
                return trendState.TrendState;
            }
        }
    }
}
