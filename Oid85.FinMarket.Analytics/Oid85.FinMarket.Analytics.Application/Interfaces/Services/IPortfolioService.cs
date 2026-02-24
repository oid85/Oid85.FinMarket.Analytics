using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис позиций портфеля
    /// </summary>
    public interface IPortfolioService
    {
        /// <summary>
        /// Редактировать позицию портфеля
        /// </summary>
        Task<EditPortfolioPositionResponse> EditPortfolioPositionAsync(EditPortfolioPositionRequest request);

        /// <summary>
        /// Получить список позиций портфеля
        /// </summary>
        Task<GetPortfolioPositionListResponse> GetPortfolioPositionListAsync(GetPortfolioPositionListRequest request);
    }
}
