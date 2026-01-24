using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис CompareTrend
    /// </summary>
    public interface ICompareTrendService
    {
        /// <summary>
        /// Сравнение трендов
        /// </summary>
        Task<GetCompareTrendResponse> GetCompareTrendAsync(GetCompareTrendRequest request);
    }
}
