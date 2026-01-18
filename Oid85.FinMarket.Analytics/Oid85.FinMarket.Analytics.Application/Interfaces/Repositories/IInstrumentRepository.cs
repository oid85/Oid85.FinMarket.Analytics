using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Repositories
{
    public interface IInstrumentRepository
    {
        Task<List<Instrument>?> GetInstrumentsAsync();
        Task<Instrument?> GetInstrumentByIdAsync(Guid id);
        Task<Guid?> EditInstrumentAsync(Instrument model);
        Task DeleteByTickerAsync(string ticker);
        Task<Guid?> AddAsync(Instrument instrument);
    }
}
