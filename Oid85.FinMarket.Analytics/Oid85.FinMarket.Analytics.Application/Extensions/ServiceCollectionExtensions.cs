using System.Linq.Expressions;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oid85.FinMarket.Analytics.Application.Factories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Factories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Application.Services;
using Oid85.FinMarket.Analytics.Application.Services.Life;

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
        services.AddScoped<IFundamentalScoreService, FundamentalScoreService>();
        services.AddScoped<IMacroService, MacroService>();
        services.AddScoped<IPortfolioService, PortfolioService>();
        services.AddScoped<IPortfolioBacktestService, PortfolioBacktestService>();
        services.AddScoped<ILifePortfolioService, LifePortfolioService>();
        services.AddScoped<IBondLifePortfolioService, BondLifePortfolioService>();
        services.AddScoped<IThreeEtfLifePortfolioService, ThreeEtfLifePortfolioService>();
        services.AddScoped<ISevenEtfLifePortfolioService, SevenEtfLifePortfolioService>();
        services.AddScoped<IShareLifePortfolioService, ShareLifePortfolioService>();
        services.AddScoped<IBondAnalyseService, BondAnalyseService>();
        services.AddScoped<IDiagramService, DiagramService>();
        services.AddScoped<IFundamentalParameterRatioService, FundamentalParameterRatioService>();
        services.AddScoped<IBulletinService, BulletinService>();
        services.AddScoped<IBlogService, BlogService>();
        services.AddTransient<IJobService, JobService>();

        services.AddScoped<IAnalyseParameterFactory, AnalyseParameterFactory>();
    }

    public static async Task RegisterHangfireJobs(
    this IHost host,
    IConfiguration configuration)
    {
        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        await using var scope = scopeFactory.CreateAsyncScope();
        var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();

        RegisterJob("SyncInstruments", () => jobService.SyncInstrumentsAsync());

        void RegisterJob(string configurationSection, Expression<Func<Task>> methodCall)
        {
            bool enable = configuration.GetValue<bool>($"Hangfire:{configurationSection}:Enable");
            string jobId = configuration.GetValue<string>($"Hangfire:{configurationSection}:JobId")!;
            string cron = configuration.GetValue<string>($"Hangfire:{configurationSection}:Cron")!;

            if (enable)
                RecurringJob.AddOrUpdate(jobId, methodCall, cron);
        }
    }
}