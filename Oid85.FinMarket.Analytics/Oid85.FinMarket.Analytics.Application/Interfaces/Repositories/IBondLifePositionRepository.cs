using Oid85.FinMarket.Analytics.Core.Models.Life;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Repositories
{
    public interface IBondLifePositionRepository
    {
        Task<List<BondLifePosition>> GetAsync();
    }
}
