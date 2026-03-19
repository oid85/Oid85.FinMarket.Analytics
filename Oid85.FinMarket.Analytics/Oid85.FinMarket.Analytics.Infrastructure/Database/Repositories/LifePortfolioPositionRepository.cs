using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class LifePortfolioPositionRepository(
        IDbContextFactory<FinMarketContext> contextFactory) 
        : ILifePortfolioPositionRepository
    {
        public async Task AddLifePortfolioPositionAsync(LifePortfolioPosition lifePosition)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entity = await context.LifePortfolioPositionEntities.FirstOrDefaultAsync(x => x.Ticker == lifePosition.Ticker);

            if (entity is not null)
                await EditLifePortfolioPositionAsync(lifePosition.Ticker, lifePosition.Size);

            else
            {
                entity = new LifePortfolioPositionEntity
                {
                    Id = Guid.NewGuid(),
                    Ticker = lifePosition.Ticker,
                    Name = lifePosition.Name,
                    Size = lifePosition.Size,
                    IsDeleted = false
                };

                await context.AddAsync(entity);
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteAllLifePortfolioPositionAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            await context.LifePortfolioPositionEntities
                .ExecuteUpdateAsync(x => x
                        .SetProperty(entity => entity.IsDeleted, true));

            await context.SaveChangesAsync();
        }

        public async Task EditLifePortfolioPositionAsync(string ticker, int size)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            await context.LifePortfolioPositionEntities
                .Where(x => x.Ticker == ticker)
                .ExecuteUpdateAsync(x => x
                        .SetProperty(entity => entity.Size, size)
                        .SetProperty(entity => entity.IsDeleted, false));

            await context.SaveChangesAsync();
        }

        public async Task<List<LifePortfolioPosition>> GetLifePortfolioPositionsAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.LifePortfolioPositionEntities.ToListAsync();

            if (entities is null)
                return [];

            var models = entities
                .Select(x =>
                    new LifePortfolioPosition
                    {
                        Id = x.Id,
                        Ticker = x.Ticker,
                        Name = x.Name,
                        Size = x.Size,
                        IsDeleted = x.IsDeleted
                    })
                .ToList();

            return models;
        }
    }
}
