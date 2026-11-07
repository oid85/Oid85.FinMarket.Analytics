using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Repositories
{
    public interface IShareLifePositionRepository
    {
        Task<List<LifePosition>> GetAsync();
        Task AddAsync(List<LifePosition> models);
        Task DeleteAsync();
    }
}
