using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис трендов
    /// </summary>
    public interface ITrendService
    {
        /// <summary>
        /// Динамика трендов
        /// </summary>
        Task<GetTrendDynamicResponse> GetTrendDynamicAsync(GetTrendDynamicRequest request);
    }
}
