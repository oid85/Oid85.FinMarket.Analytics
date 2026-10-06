using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services.Life
{
    public class SevenEtfLifePortfolioService(
        IDataService dataService,
        IInstrumentService instrumentService,
        ISevenEtfLifePositionRepository positionRepository) 
        : ISevenEtfLifePortfolioService
    {
        public Task<LifePortfolioResponse> GetPositionListAsync(LifePortfolioRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
