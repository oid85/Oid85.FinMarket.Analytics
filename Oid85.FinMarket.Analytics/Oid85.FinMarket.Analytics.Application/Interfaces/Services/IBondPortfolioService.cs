using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    /// <summary>
    /// Сервис позиций портфеля облигаций
    /// </summary>
    public interface IBondPortfolioService
    {
        /// <summary>
        /// Редактировать позицию портфеля
        /// </summary>
        Task<EditBondPortfolioPositionResponse> EditBondPortfolioPositionAsync(EditBondPortfolioPositionRequest request);

        /// <summary>
        /// Редактировать сумму портфеля
        /// </summary>
        Task<EditBondPortfolioTotalSumResponse> EditBondPortfolioTotalSumAsync(EditBondPortfolioTotalSumRequest request);

        /// <summary>
        /// Получить список позиций портфеля
        /// </summary>
        Task<GetBondPortfolioPositionListResponse> GetBondPortfolioPositionListAsync(GetBondPortfolioPositionListRequest request);
    }
}
