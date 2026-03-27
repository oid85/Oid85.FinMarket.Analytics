using Oid85.FinMarket.Analytics.Application.Helpers;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class DiagramService(
        IInstrumentService instrumentService,
        IDataService dataService,
        IWeekTrendService weekTrendService) 
        : IDiagramService
    {
        public async Task<GetClosePriceDiagramResponse> GetClosePriceDiagramAsync(GetClosePriceDiagramRequest request)
        {
            var getWeekDeltaResponse = await weekTrendService.GetWeekDeltaAsync(new() { LastWeeksCount = 10 });
            
            var allInstruments = (await instrumentService.GetAnalyticInstrumentListAsync(new() { LastDaysCount = 90 })).Instruments
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

            var closePriceDiagramData = await dataService.GetClosePriceDiagramDataAsync(tickers);
            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);

            foreach (var instrument in instruments)
                response.Items.Add(new GetClosePriceDiagramItemResponse()
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    InPortfolio = instrument.InPortfolio,
                    Data = [.. ultimateSmootherData[instrument.Ticker].Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddMonths(-6))).Select(x => new GetClosePriceDiagramDateValueResponse { Date = x.Date, Value = x.Value })],
                    TrendState = TrendStateHelper.GetTrendState(ultimateSmootherData[instrument.Ticker]).Message
                });

            return response;
        }
    }
}
