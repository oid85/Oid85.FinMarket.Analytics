using Microsoft.Extensions.DependencyInjection;

namespace Oid85.FinMarket.Analytics.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        // services.AddTransient<IInstrumentService, InstrumentService>();
    }
}