using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses.ApiClient;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients
{
    /// <summary>
    /// Клиент сервиса FinMarket.TraderTinkoff
    /// </summary>
    public interface ITraderTinkoffEldApiClient
    {
        /// <summary>
        /// Получить информацию по портфелю
        /// </summary>
        Task<PortfolioInfoResponse> GetPortfolioInfoAsync(TinkoffPortfolioInfoRequest request);
    }
}
