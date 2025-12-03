using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oid85.FinMarket.Analytics.Common.KnownConstants;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {    
        services.AddDbContextPool<FinMarketContext>((serviceProvider, options) =>
        {  
            options.UseNpgsql(configuration.GetValue<string>(KnownSettingsKeys.PostgresFinMarketAnalyticsConnectionString)!);
        });

        services.AddPooledDbContextFactory<FinMarketContext>(options =>
            options
                .UseNpgsql(configuration.GetValue<string>(KnownSettingsKeys.PostgresFinMarketAnalyticsConnectionString)!)
                .EnableServiceProviderCaching(false), poolSize: 32);
    }

    public static async Task ApplyMigrations(this IHost host)
    {
        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        await using var scope = scopeFactory.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<FinMarketContext>();
        await context.Database.MigrateAsync();
    }
}