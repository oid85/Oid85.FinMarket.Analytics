using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services.Life
{
    public class LifeSuperPortfolioService(
        ILifePortfolioRepository lifePortfolioRepository)
        : ILifeSuperPortfolioService
    {
        public Task<LifeSuperPortfolioResponse> GetLifeSuperPortfolioAsync(LifeSuperPortfolioRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
