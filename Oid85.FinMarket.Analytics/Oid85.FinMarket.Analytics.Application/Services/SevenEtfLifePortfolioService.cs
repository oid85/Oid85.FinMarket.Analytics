using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests.Life;
using Oid85.FinMarket.Analytics.Core.Responses.Life;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class SevenEtfLifePortfolioService : ISevenEtfLifePortfolioService
    {
        public Task<SevenEtfLifePortfolioResponse> GetPositionListAsync(SevenEtfLifePortfolioRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
