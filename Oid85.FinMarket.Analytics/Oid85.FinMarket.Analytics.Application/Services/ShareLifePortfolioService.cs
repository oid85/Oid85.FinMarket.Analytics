using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class ShareLifePortfolioService : IShareLifePortfolioService
    {
        public Task<ShareLifePortfolioPositionListResponse> GetPositionListAsync(ShareLifePortfolioPositionListRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
