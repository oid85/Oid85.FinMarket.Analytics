using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class ThreeEtfLifePortfolioPositionRepository(
        IDbContextFactory<FinMarketContext> contextFactory)
        : IThreeEtfLifePortfolioPositionRepository
    {
        public async Task<List<ThreeEtfLifePortfolioPosition>> GetAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.ThreeEtfLifePortfolioPositionEntities.ToListAsync();

            if (entities is null)
                return [];

            var models = entities
                .Select(x =>
                    new ThreeEtfLifePortfolioPosition
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
