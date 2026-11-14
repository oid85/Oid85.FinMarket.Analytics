using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Repositories
{
    public interface IMoneyRepository
    {
        Task<List<MoneyDateValue>> GetAsync(string portfolioName);
        Task AddAsync(List<MoneyDateValue> models);
    }
}
