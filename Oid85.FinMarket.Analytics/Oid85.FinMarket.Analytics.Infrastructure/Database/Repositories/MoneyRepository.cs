using Google.Rpc.Context;
using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class MoneyRepository(
        IDbContextFactory<FinMarketContext> contextFactory)
        : IMoneyRepository
    {
        public async Task<List<MoneyDateValue>> GetAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var from = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));
            var to = DateOnly.FromDateTime(DateTime.Today);

            var entities = await context.MoneyDateValueEntities
                .Where(x => x.Date >= from && x.Date <= to)
                .AsNoTracking().ToListAsync();

            return [.. entities
                .Select(x =>
                new MoneyDateValue
                {
                    Date = x.Date,
                    Money = x.Money,
                    PortfolioName = x.PortfolioName
                })];
        }

        public async Task AddAsync(List<MoneyDateValue> models)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.MoneyDateValueEntities.AsNoTracking().ToListAsync();

            foreach (var model in models)
            {
                if (!entities.Any(x => x.Date == model.Date && x.PortfolioName == model.PortfolioName))
                {
                    await context.AddAsync(
                        new MoneyDateValueEntity
                        {
                            Date = model.Date,
                            Money = model.Money,
                            PortfolioName = model.PortfolioName
                        });

                    await context.SaveChangesAsync();
                }
            }            
        }
    }
}
