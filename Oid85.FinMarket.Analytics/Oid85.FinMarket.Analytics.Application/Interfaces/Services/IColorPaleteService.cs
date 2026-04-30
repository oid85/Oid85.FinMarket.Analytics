
namespace Oid85.FinMarket.Analytics.Application.Interfaces.Services
{
    public interface IColorPaleteService
    {
        Task<(string Color, string Description)> GetColorPeAsync(string ticker, string period);
        Task<(string Color, string Description)> GetColorPbvAsync(string ticker, string period);
    }
}
