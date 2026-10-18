using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses.ApiClient;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients
{
    /// <summary>
    /// Клиент сервиса FinMarket.TraderFinam
    /// </summary>
    public interface ITraderFinamApiClient
    {
        /// <summary>
        /// Получить цены по тикеру
        /// </summary>
        Task<GetInstrumentPriceResponse> GetInstrumentPriceAsync(GetInstrumentPriceRequest request);
    }
}
