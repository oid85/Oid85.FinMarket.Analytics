using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Repositories
{
    public interface IThreeEtfLifePositionRepository
    {
        Task<List<LifePosition>> GetAsync();
    }
}
