using System.Text.Json.Serialization;
using Oid85.FinMarket.Analytics.Application.Extensions;
using Oid85.FinMarket.Analytics.Common.Converters;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Infrastructure.Extensions;
using Oid85.FinMarket.Analytics.WebHost.Extensions;

namespace Oid85.FinMarket.Analytics.WebHost
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;
                });

            builder.Services.AddMemoryCache();
            builder.Services.ConfigureLogger();
            builder.Services.ConfigureSwagger(builder.Configuration);
            builder.Services.ConfigureCors(builder.Configuration);
            builder.Services.ConfigureHangfire();
            builder.Services.ConfigureApplicationServices();
            builder.Services.ConfigureDatabase(builder.Configuration);
            builder.Services.ConfigureFinMarketStorageServiceApiClient(builder.Configuration);

            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "Oid85.FinMarket.Analytics";
            });

            bool applyMigrations = builder.Configuration.GetValue<bool>(KnownSettingsKeys.PostgresApplyMigrationsOnStart);
            int port = builder.Configuration.GetValue<int>(KnownSettingsKeys.DeployPort);

            var app = builder.Build();

            if (applyMigrations)
                await app.ApplyMigrations();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
            });

            app.MapControllers();

            app.Urls.Add($"http://0.0.0.0:{port}");

            await app.RunAsync();
        }
    }
}
