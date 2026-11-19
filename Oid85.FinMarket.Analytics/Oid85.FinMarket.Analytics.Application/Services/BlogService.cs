using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class BlogService() 
        : IBlogService
    {
        public async Task<CreateWeekTradesPostResponse> CreateWeekTradesPostAsync(CreateWeekTradesPostRequest request)
        {
            return new();
        }
    }
}
