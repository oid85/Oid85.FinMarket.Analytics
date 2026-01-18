using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities;
using Oid85.FinMarket.Analytics.Infrastructure.Database.Schemas;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database;

public class FinMarketContext(DbContextOptions<FinMarketContext> options) : DbContext(options)
{
    public DbSet<InstrumentEntity> InstrumentEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .HasDefaultSchema(KnownDatabaseSchemas.Default)
            .ApplyConfigurationsFromAssembly(
                typeof(FinMarketContext).Assembly,
                type => type
                    .GetInterface(typeof(IFinMarketSchema).ToString()) != null)
            .UseIdentityAlwaysColumns();
    }    
}