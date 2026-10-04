using Oid85.FinMarket.Analytics.Application.Interfaces.Services;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class JobService(
        IInstrumentService instrumentService)
        : IJobService
    {
        public Task SyncInstrumentsAsync() =>
            instrumentService.SyncInstrumentListAsync();
    }
}
