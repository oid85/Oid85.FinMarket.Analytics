using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class LifePortfolioRepository(
        IDbContextFactory<FinMarketContext> contextFactory)
        : ILifePortfolioRepository
    {
        public async Task<List<LifePortfolio>> GetAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.LifePortfolioEntities.ToListAsync();

            if (entities is null)
                return [];

            var models = entities
                .Select(x =>
                    new LifePortfolio
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description, 
                        Weight = x.Weight
                    })
                .OrderByDescending(x => x.Weight)
                .ToList();

            return models;
        }
    }
}
