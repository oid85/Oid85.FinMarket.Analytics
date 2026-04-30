using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Interfaces.Factories
{
    public interface IFundamentalParameterFactory
    {
        Task<Parameter<double?>?> CreatePeAsync(string ticker, string period);
        Task<Parameter<double?>?> CreatePbvAsync(string ticker, string period);
    }
}
