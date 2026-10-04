using Oid85.FinMarket.Analytics.Core.Models.Life;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Repositories
{
    public interface ISevenEtfLifePositionRepository
    {
        Task<List<SevenEtfLifePosition>> GetAsync();
    }
}
