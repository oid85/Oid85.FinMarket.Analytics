using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис анализа облигаций
    /// </summary>
    public interface IBondAnalyseService
    {
        /// <summary>
        /// Получить анализ по облигациям
        /// </summary>
        Task<GetBondAnalyseResponse> GetBondAnalyseAsync(GetBondAnalyseRequest request);
    }
}
