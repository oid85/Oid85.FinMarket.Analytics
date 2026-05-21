using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class PortfolioRebalanceService 
        : IPortfolioRebalanceService
    {
        public async Task<PortfolioRebalanceResponse> PortfolioRebalanceAsync(PortfolioRebalanceRequest request)
        {
            var response = new PortfolioRebalanceResponse();



            return response;
        }
    }
}
