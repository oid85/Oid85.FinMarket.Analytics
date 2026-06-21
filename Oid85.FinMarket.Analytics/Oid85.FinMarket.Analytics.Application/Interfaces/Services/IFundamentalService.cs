using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис фундаментальных параметров
    /// </summary>
    public interface IFundamentalService
    {
        /// <summary>
        /// Создание/изменение фундаментального параметра
        /// </summary>
        Task<CreateOrUpdateAnalyticFundamentalParameterResponse> CreateOrUpdateFundamentalParameterAsync(CreateOrUpdateAnalyticFundamentalParameterRequest request);

        /// <summary>
        /// Удаление фундаментального параметра
        /// </summary>
        Task<DeleteAnalyticFundamentalParameterResponse> DeleteFundamentalParameterAsync(DeleteAnalyticFundamentalParameterRequest request);

        /// <summary>
        /// Получить список фундаментальных параметров
        /// </summary>
        Task<GetAnalyticFundamentalParameterListResponse> GetFundamentalParameterListAsync(GetAnalyticFundamentalParameterListRequest request);

        /// <summary>
        /// Получить фундаментальные параметры по сектору
        /// </summary>
        Task<GetFundamentalBySectorResponse> GetFundamentalBySectorAsync(GetFundamentalBySectorRequest request);

        /// <summary>
        /// Получить фундаментальные параметры по компании
        /// </summary>
        Task<GetFundamentalByCompanyResponse> GetFundamentalByCompanyAsync(GetFundamentalByCompanyRequest request);

        /// <summary>
        /// Получить рейтинг по фундаментальным данным
        /// </summary>
        Task<GetFundamentalRatingListResponse> GetFundamentalRatingListAsync(GetFundamentalRatingListRequest request);
    }
}
