using Oid85.FinMarket.Analytics.Application.Interfaces.Services;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    public class JobService(
        IInstrumentService instrumentService,
        ILifeSuperPortfolioService lifeSuperPortfolioService)
        : IJobService
    {
        public Task SaveMoneyAsync() =>
            lifeSuperPortfolioService.SaveMoneyAsync();

        public Task SyncInstrumentsAsync() =>
            instrumentService.SyncInstrumentListAsync();
    }
}
