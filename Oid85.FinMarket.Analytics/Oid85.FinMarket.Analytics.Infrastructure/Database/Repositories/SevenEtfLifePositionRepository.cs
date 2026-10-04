using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models.Life;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class SevenEtfLifePositionRepository(
        IDbContextFactory<FinMarketContext> contextFactory)
        : ISevenEtfLifePositionRepository
    {
        public async Task<List<SevenEtfLifePosition>> GetAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.SevenEtfLifePositionEntities.ToListAsync();

            if (entities is null)
                return [];

            var models = entities
                .Select(x =>
                    new SevenEtfLifePosition
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
