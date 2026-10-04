using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests.Life;
using Oid85.FinMarket.Analytics.Core.Responses.Life;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class ThreeEtfLifePortfolioService : IThreeEtfLifePortfolioService
    {
        public Task<ThreeEtfLifePortfolioResponse> GetPositionListAsync(ThreeEtfLifePortfolioRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
