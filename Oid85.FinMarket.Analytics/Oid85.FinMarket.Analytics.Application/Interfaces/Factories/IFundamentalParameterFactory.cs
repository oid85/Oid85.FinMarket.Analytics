using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Factories
{
    public interface IFundamentalParameterFactory
    {
        Task<AnalyseParameter<double?>?> CreatePeAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreatePbvAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateRevenueAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateNetProfitAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateFcfAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateEpsAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateNetDebtAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateRoaAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateRoeAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateEvEbitdaAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateNetDebtEbitdaAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateEbitdaRevenueAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateDividendYieldAsync(string ticker, string period);
        Task<AnalyseParameter<double?>?> CreateDeltaMinMaxAsync(string ticker, string period);
    }
}
