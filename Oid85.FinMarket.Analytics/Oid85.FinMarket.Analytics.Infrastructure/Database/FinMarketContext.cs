using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities;
using Oid85.FinMarket.Analytics.Infrastructure.Database.Schemas;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database;

public class FinMarketContext(DbContextOptions<FinMarketContext> options) : DbContext(options)
{
    public DbSet<InstrumentEntity> InstrumentEntities { get; set; }
    public DbSet<ParameterEntity> ParameterEntities { get; set; }
    public DbSet<LifePortfolioPositionEntity> LifePortfolioPositionEntities { get; set; }
    public DbSet<ShareLifePositionEntity> ShareLifePositionEntities { get; set; }
	public DbSet<BondLifePositionEntity> BondLifePositionEntities { get; set; }
	public DbSet<SevenEtfLifePositionEntity> SevenEtfLifePositionEntities { get; set; }
	public DbSet<ThreeEtfLifePositionEntity> ThreeEtfLifePositionEntities { get; set; }

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