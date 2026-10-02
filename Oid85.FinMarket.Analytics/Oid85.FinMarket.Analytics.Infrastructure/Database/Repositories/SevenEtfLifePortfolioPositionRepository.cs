using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class SevenEtfLifePortfolioPositionRepository(
        IDbContextFactory<FinMarketContext> contextFactory)
        : ISevenEtfLifePortfolioPositionRepository
    {
        public async Task<List<SevenEtfLifePortfolioPosition>> GetAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.SevenEtfLifePortfolioPositionEntities.ToListAsync();

            if (entities is null)
                return [];

            var models = entities
                .Select(x =>
                    new SevenEtfLifePortfolioPosition
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
