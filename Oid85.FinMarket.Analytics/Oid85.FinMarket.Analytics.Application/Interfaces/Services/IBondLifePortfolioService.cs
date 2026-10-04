using Oid85.FinMarket.Analytics.Core.Requests.Life;
using Oid85.FinMarket.Analytics.Core.Responses.Life;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    public interface IBondLifePortfolioService
    {
        Task<BondLifePortfolioResponse> GetPositionListAsync(BondLifePortfolioRequest request);
    }
}
