using System.Timers;
using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class BondLifePositionRepository(
        IDbContextFactory<FinMarketContext> contextFactory)
        : IBondLifePositionRepository
    {
        public async Task AddAsync(List<LifePosition> models)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = models
                .Select(x =>
                new BondLifePositionEntity
                {
                    Id = Guid.NewGuid(),
                    Ticker = x.Ticker,
                    Name = x.Name,
                    Size = x.Size,
                    Weight = x.Weight
                })
                .ToList();

            await context.AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            await context.BondLifePositionEntities.ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        }

        public async Task<List<LifePosition>> GetAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.BondLifePositionEntities.ToListAsync();

            if (entities is null)
                return [];

            var models = entities
                .Select(x =>
                    new LifePosition
                    {
                        Id = x.Id,
                        Ticker = x.Ticker,
                        Name = x.Name,
                        Size = x.Size, 
                        Price = x.Price, 
                        Weight = x.Weight
                    })
                .OrderBy(x => x.Ticker)
                .ToList();

            return models;
        }
    }
}
