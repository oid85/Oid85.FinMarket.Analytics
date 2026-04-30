using Microsoft.Extensions.DependencyInjection;
using Oid85.FinMarket.Analytics.Application.Factories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Factories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Application.Services;

namespace Oid85.FinMarket.Analytics.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<ITrendDynamicService, TrendDynamicService>();
        services.AddScoped<IWeekTrendService, WeekTrendService>();
        services.AddScoped<ICompareTrendService, CompareTrendService>();
        services.AddScoped<IInstrumentService, InstrumentService>();
        services.AddScoped<IFundamentalService, FundamentalService>();
        services.AddScoped<IMacroService, MacroService>();
        services.AddScoped<IPortfolioService, PortfolioService>();
        services.AddScoped<IBondPortfolioService, BondPortfolioService>();
        services.AddScoped<ILifePortfolioService, LifePortfolioService>();
        services.AddScoped<IBondAnalyseService, BondAnalyseService>();
        services.AddScoped<IDiagramService, DiagramService>();
        services.AddScoped<IColorPaleteService, ColorPaleteService>();

        services.AddScoped<IFundamentalParameterFactory, FundamentalParameterFactory>();
    }
}