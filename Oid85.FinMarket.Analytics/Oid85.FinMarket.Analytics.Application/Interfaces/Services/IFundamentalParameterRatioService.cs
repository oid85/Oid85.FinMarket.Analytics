
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    public interface IFundamentalParameterRatioService
    {
        Task<FundamentalParameterRatio> GetRatioMarketCapAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioPeAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioPbvAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioRevenueAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioNetProfitAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioFcfAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetColorEpsAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioNetDebtAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioRoaAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioRoeAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioEvEbitdaAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioNetDebtEbitdaAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioDebtRatioAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioDebtEquityAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioEbitdaRevenueAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioDividendYieldAsync(string ticker, string period);
        Task<FundamentalParameterRatio> GetRatioDeltaMinMaxAsync(string ticker, string period);
    }
}
