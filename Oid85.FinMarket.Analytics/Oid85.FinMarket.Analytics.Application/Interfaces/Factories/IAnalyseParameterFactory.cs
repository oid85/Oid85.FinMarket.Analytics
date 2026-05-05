using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Factories
{
    public interface IAnalyseParameterFactory
    {
        Task<AnalyseRatioParameter<double?>?> CreatePeAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreatePbvAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateRevenueAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateNetProfitAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateFcfAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateEpsAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateNetDebtAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateRoaAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateRoeAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateEvEbitdaAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateNetDebtEbitdaAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateEbitdaRevenueAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateOwnCapitalNumberSharesAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateDividendYieldAsync(string ticker, string period);
        Task<AnalyseRatioParameter<double?>?> CreateDeltaMinMaxAsync(string ticker, string period);
        Task<AnalyseRatioParameter<bool?>?> CreateDividendAristocratAsync(string ticker);
    }
}
