using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class BulletinService : IBulletinService
    {
        public Task<GetBulletinResponse> GetBulletinAsync(GetBulletinRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
