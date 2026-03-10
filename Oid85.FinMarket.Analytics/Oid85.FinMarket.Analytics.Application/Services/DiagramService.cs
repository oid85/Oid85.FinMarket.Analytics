using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class DiagramService(
        IInstrumentService instrumentService,
        IDataService dataService) 
        : IDiagramService
    {
        public async Task<GetClosePriceDiagramResponse> GetClosePriceDiagramAsync(GetClosePriceDiagramRequest request)
        {
            var instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new() { LastDaysCount = 90 })).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();

            var response = new GetClosePriceDiagramResponse();

            var data = await dataService.GetClosePriceDiagramDataAsync(tickers);

            foreach (var instrument in instruments)
            {
                response.Items.Add(new GetClosePriceDiagramItemResponse()
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    Data = [.. data[instrument.Ticker].Select(x => new GetClosePriceDiagramDateValueResponse { Date = x.Date, Value = x.Value})]
                });
            }

            return response;
        }
    }
}
