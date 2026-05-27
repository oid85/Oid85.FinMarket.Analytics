using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    public interface IPortfolioBacktestService
    {
        Task<PortfolioBacktestResponse> PortfolioBacktestAsync(PortfolioBacktestRequest request);
    }
}
