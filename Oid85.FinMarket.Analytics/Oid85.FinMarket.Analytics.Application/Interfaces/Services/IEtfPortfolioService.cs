using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис позиций портфеля ETF
    /// </summary>
    public interface IEtfPortfolioService
    {
        /// <summary>
        /// Редактировать позицию портфеля
        /// </summary>
        Task<EditEtfPortfolioPositionResponse> EditPortfolioPositionAsync(EditEtfPortfolioPositionRequest request);

        /// <summary>
        /// Получить список позиций портфеля
        /// </summary>
        Task<GetEtfPortfolioPositionListResponse> GetPortfolioPositionListAsync(GetEtfPortfolioPositionListRequest request);
    }
}
