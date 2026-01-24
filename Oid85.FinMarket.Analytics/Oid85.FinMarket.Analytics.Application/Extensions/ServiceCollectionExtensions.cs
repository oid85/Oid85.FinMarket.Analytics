using Microsoft.Extensions.DependencyInjection;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Application.Services;

namespace Oid85.FinMarket.Analytics.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        services.AddTransient<IDataService, DataService>();
        services.AddTransient<ITrendDynamicService, TrendDynamicService>();
        services.AddTransient<ICompareTrendService, CompareTrendService>();
        services.AddTransient<IInstrumentService, InstrumentService>();
    }
}