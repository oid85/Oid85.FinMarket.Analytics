using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses.ApiClient;
using Oid85.FinMarket.Storage.Core.Requests;
using Oid85.FinMarket.Storage.Core.Responses;

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

        /// <summary>
        /// Получить последнюю свечу
        /// </summary>
        Task<GetLastCandleResponse> GetLastCandleAsync(GetLastCandleRequest request);

        /// <summary>
        /// Получить фундаментальные параметры
        /// </summary>
        Task<GetFundamentalParameterListResponse> GetFundamentalParameterListAsync(GetFundamentalParameterListRequest request);

        /// <summary>
        /// Создать или редактировать фундаментальные параметры
        /// </summary>
        Task<CreateOrUpdateFundamentalParameterResponse> CreateOrUpdateFundamentalParameterAsync(CreateOrUpdateFundamentalParameterRequest request);

        /// <summary>
        /// Получить именения индекса потребительских цен
        /// </summary>
        Task<GetConsumerPriceIndexChangeListResponse> GetConsumerPriceIndexChangeListAsync(GetConsumerPriceIndexChangeListRequest request);

        /// <summary>
        /// Создать или редактировать именение индекса потребительских цен
        /// </summary>
        Task<CreateOrUpdateConsumerPriceIndexChangeResponse> CreateOrUpdateConsumerPriceIndexChangeAsync(CreateOrUpdateConsumerPriceIndexChangeRequest request);

        /// <summary>
        /// Получить значения денежных агрегатов
        /// </summary>
        Task<GetMonetaryAggregateListResponse> GetMonetaryAggregateListAsync(GetMonetaryAggregateListRequest request);

        /// <summary>
        /// Создать или редактировать денежного агрегата
        /// </summary>
        Task<CreateOrUpdateMonetaryAggregateResponse> CreateOrUpdateMonetaryAggregateAsync(CreateOrUpdateMonetaryAggregateRequest request);

        /// <summary>
        /// Получить значения ключевой ставки
        /// </summary>
        Task<GetKeyRateListResponse> GetKeyRateListAsync(GetKeyRateListRequest request);

        /// <summary>
        /// Создать или редактировать значение ключевой ставки
        /// </summary>
        Task<CreateOrUpdateKeyRateResponse> CreateOrUpdateKeyRateAsync(CreateOrUpdateKeyRateRequest request);
    }
}
