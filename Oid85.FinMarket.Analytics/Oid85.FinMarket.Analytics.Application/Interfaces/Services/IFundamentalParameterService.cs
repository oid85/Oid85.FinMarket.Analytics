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
        /// Удаление фундаментального параметра
        /// </summary>
        Task<DeleteAnalyticFundamentalParameterResponse> DeleteAnalyticFundamentalParameterAsync(DeleteAnalyticFundamentalParameterRequest request);

        /// <summary>
        /// Пузырьковая диаграмма
        /// </summary>
        Task<GetAnalyticFundamentalParameterBubbleDiagramResponse> GetAnalyticFundamentalParameterBubbleDiagramAsync(GetAnalyticFundamentalParameterBubbleDiagramRequest request);

        /// <summary>
        /// Получить список фундаментальных параметров
        /// </summary>
        Task<GetAnalyticFundamentalParameterListResponse> GetAnalyticFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request);        

        /// <summary>
        /// Получить фундаментальные параметры по сектору
        /// </summary>
        Task<GetFundamentalBySectorResponse> GetFundamentalBySectorAsync(GetFundamentalBySectorRequest request);

        /// <summary>
        /// Получить фундаментальные параметры по компании
        /// </summary>
        Task<GetFundamentalByCompanyResponse> GetFundamentalByCompanyAsync(GetFundamentalByCompanyRequest request);        
    }
}
