using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Factories
{
    public interface IFundamentalParameterFactory
    {
        Task<Parameter<double?>?> CreatePeAsync(string ticker, string period);
        Task<Parameter<double?>?> CreatePbvAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateRevenueAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateNetProfitAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateFcfAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateEpsAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateNetDebtAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateRoaAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateRoeAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateEvEbitdaAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateNetDebtEbitdaAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateEbitdaRevenueAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateDividendYieldAsync(string ticker, string period);
        Task<Parameter<double?>?> CreateDeltaMinMaxAsync(string ticker, string period);
    }
}
