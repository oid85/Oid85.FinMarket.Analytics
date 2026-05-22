using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class PortfolioRebalanceService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        IPortfolioService portfolioService,
        IInstrumentService instrumentService,
        IDataService dataService)
        : IPortfolioRebalanceService
    {
        public async Task<PortfolioRebalanceResponse> PortfolioRebalanceAsync(PortfolioRebalanceRequest request)
        {
            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var positions = (await portfolioService.GetPortfolioPositionListAsync(new GetPortfolioPositionListRequest())).PortfolioPositions;



            var response = new PortfolioRebalanceResponse();



            return response;
        }
    }
}
