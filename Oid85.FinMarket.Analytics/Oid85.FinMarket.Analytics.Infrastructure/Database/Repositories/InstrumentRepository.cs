using Google.Api;
using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class InstrumentRepository(
        IDbContextFactory<FinMarketContext> contextFactory) 
        : IInstrumentRepository
    {
        public async Task<Guid?> AddAsync(Instrument instrument)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entity = await context.InstrumentEntities.FirstOrDefaultAsync(x => x.Ticker == instrument.Ticker);

            if (entity is not null)
                return null;

            entity = new InstrumentEntity
            {
                Id = Guid.NewGuid(),
                Ticker = instrument.Ticker,
                Name = instrument.Name,
                Type = instrument.Type,
                IsSelected = instrument.IsSelected
            };

            await context.AddAsync(entity);
            await context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task DeleteByTickerAsync(string ticker)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            await context.InstrumentEntities.Where(x => x.Ticker == ticker).ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        }

        public async Task<Guid?> EditInstrumentAsync(Instrument model)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            await context.InstrumentEntities
                .Where(x => x.Id == model.Id)
                .ExecuteUpdateAsync(x => x
                        .SetProperty(entity => entity.Ticker, model.Ticker)
                        .SetProperty(entity => entity.Name, model.Name)
                        .SetProperty(entity => entity.Type, model.Type)
                        .SetProperty(entity => entity.IsSelected, model.IsSelected));

            await context.SaveChangesAsync();

            return model.Id;
        }

        public async Task<Instrument?> GetInstrumentByIdAsync(Guid id)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entity = await context.InstrumentEntities.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
                return null;

            var model = new Instrument
            {
                Id = entity.Id,
                Ticker = entity.Ticker,
                Name = entity.Name,
                Type = entity.Type,
                IsSelected = entity.IsSelected
            };

            return model;
        }

        public async Task<List<Instrument>?> GetInstrumentsAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.InstrumentEntities.ToListAsync();

            if (entities is null)
                return null;

            var models = entities
                .Select(x =>
                    new Instrument
                    {
                        Id = x.Id,
                        Ticker = x.Ticker,
                        Name = x.Name,
                        Type = x.Type,
                        IsSelected = x.IsSelected
                    })
                .ToList();

            return models;
        }
    }
}
