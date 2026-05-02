using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    public interface IFundamentalScoreService
    {
        /// <summary>
        /// Получить Score для фундаментального параметра по тикеру
        /// </summary>
        Task<FundamentalScore?> GetFundamentalScoreAsync(string ticker);
    }
}
