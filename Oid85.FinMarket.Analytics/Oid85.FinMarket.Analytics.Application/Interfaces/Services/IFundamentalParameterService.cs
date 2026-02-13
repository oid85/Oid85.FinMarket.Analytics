using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис фундаментальных параметров
    /// </summary>
    public interface IFundamentalParameterService
    {
        /// <summary>
        /// Создание/изменение фундаментального параметра
        /// </summary>
        Task<CreateOrUpdateAnalyticFundamentalParameterResponse> CreateOrUpdateAnalyticFundamentalParameterAsync(CreateOrUpdateAnalyticFundamentalParameterRequest request);

        /// <summary>
        /// Пузырьковая диаграмма
        /// </summary>
        Task<GetAnalyticFundamentalParameterBubbleDiagramResponse> GetAnalyticFundamentalParameterBubbleDiagramAsync(GetAnalyticFundamentalParameterBubbleDiagramRequest request);

        /// <summary>
        /// Получить список фундаментальных параметров
        /// </summary>
        Task<GetAnalyticFundamentalParameterListResponse> GetAnalyticFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request);
    }
}
