
namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    public interface IColorPaleteService
    {
        Task<(double Ratio, string Color, string Description)> GetColorPeAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorPbvAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorRevenueAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorNetProfitAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorFcfAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorEpsAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorNetDebtAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorRoaAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorRoeAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorEvEbitdaAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorNetDebtEbitdaAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorEbitdaRevenueAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorDividendYieldAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorDeltaMinMaxAsync(string ticker, string period);
    }
}
