using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients
{
    /// <summary>
    /// Клиент сервиса FinMarket.Storage
    /// </summary>
    public interface IFinMarketStorageServiceApiClient
    {
        /// <summary>
        /// Получить инструменты
        /// </summary>
        Task<GetInstrumentListResponse> GetInstrumentListAsync(GetInstrumentListRequest request);

        /// <summary>
        /// Получить свечи
        /// </summary>
        Task<GetCandleListResponse> GetCandleListAsync(GetCandleListRequest request);
    }
}
