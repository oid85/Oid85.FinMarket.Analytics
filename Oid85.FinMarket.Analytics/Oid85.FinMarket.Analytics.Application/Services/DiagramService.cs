using Oid85.FinMarket.Analytics.Application.Helpers;
using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class DiagramService(
        IInstrumentService instrumentService,
        IDataService dataService,
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient) 
        : IDiagramService
    {
        public async Task<GetClosePriceDiagramResponse> GetClosePriceDiagramAsync(GetClosePriceDiagramRequest request)
        {                        
            var allInstruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments
                .ToList();

            List<string> indexTickers = [ KnownIndexTickers.IMOEX, KnownIndexTickers.MCFTR, KnownIndexTickers.RGBI, KnownIndexTickers.RVI ];

            var indexes = allInstruments
                .Where(x => x.Type == KnownInstrumentTypes.Index)
                .OrderBy(x => indexTickers.Contains(x.Ticker))
                .ToList();

            var instrumentsInPortfolio = allInstruments
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => x.IsSelected)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var instrumentsNotInPortfolio = allInstruments
                .Where(x => x.Type == KnownInstrumentTypes.Share)
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

            foreach (var instrument in instruments)
            {
                items.Add(
                    new ()
                    {
                        Ticker = instrument.Ticker,
                        Name = instrument.Name,
                        InPortfolio = instrument.InPortfolio,
                        Data = [.. ultimateSmootherData[instrument.Ticker].Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddYears(-1))).Select(x => new GetClosePriceDiagramDateValueResponse { Date = x.Date, Value = x.Value })],
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
    }
}
