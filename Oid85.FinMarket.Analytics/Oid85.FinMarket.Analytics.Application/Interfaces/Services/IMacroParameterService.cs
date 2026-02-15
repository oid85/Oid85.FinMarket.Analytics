using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис макроэкономических параметров
    /// </summary>
    public interface IMacroParameterService
    {
        /// <summary>
        /// Создание/изменение макроэкономического параметра
        /// </summary>
        Task<CreateOrUpdateAnalyticMacroParameterResponse> CreateOrUpdateAnalyticMacroParameterAsync(CreateOrUpdateAnalyticMacroParameterRequest request);

        /// <summary>
        /// Получить список макроэкономических параметров
        /// </summary>
        Task<GetAnalyticMacroParameterListResponse> GetAnalyticMacroParameterListAsync(GetAnalyticMacroParameterListRequest request);
    }
}
