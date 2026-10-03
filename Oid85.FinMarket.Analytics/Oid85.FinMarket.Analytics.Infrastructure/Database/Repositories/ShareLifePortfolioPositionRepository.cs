using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class ShareLifePortfolioPositionRepository(
        IDbContextFactory<FinMarketContext> contextFactory)
        : IShareLifePortfolioPositionRepository
    {
        public async Task<List<ShareLifePortfolioPosition>> GetAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.ShareLifePortfolioPositionEntities.ToListAsync();

            if (entities is null)
                return [];

            var models = entities
                .Select(x =>
                    new ShareLifePortfolioPosition
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
