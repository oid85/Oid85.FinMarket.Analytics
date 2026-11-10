using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities;
using static Grpc.Core.Metadata;

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
                IsSelected = instrument.IsSelected,                
                ManualCoefficient = instrument.ManualCoefficient,
                Sector = "-"
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
                        .SetProperty(entity => entity.IsSelected, model.IsSelected)                        
                        .SetProperty(entity => entity.ManualCoefficient, model.ManualCoefficient)
                        .SetProperty(entity => entity.Sector, model.Sector));

            await context.SaveChangesAsync();

            return model.Id;
        }

        public async Task<Instrument?> GetInstrumentByIdAsync(Guid id)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entity = await context.InstrumentEntities.FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
                return null;

            bool inPortfolio =
                await context.BondLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker) ||
                await context.ShareLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker) ||
                await context.SevenEtfLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker) ||
                await context.ThreeEtfLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker);

            var model = new Instrument
            {
                Id = entity.Id,
                Ticker = entity.Ticker,
                Name = entity.Name,
                Type = entity.Type,
                IsSelected = entity.IsSelected,
                InPortfolio = inPortfolio,
                ManualCoefficient = entity.ManualCoefficient,
                Sector = entity.Sector
            };

            return model;
        }

        public async Task<Instrument?> GetInstrumentByTickerAsync(string ticker)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entity = await context.InstrumentEntities.FirstOrDefaultAsync(x => x.Ticker == ticker);

            if (entity is null)
                return null;

            bool inPortfolio =
                await context.BondLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker) ||
                await context.ShareLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker) ||
                await context.SevenEtfLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker) ||
                await context.ThreeEtfLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker);

            var model = new Instrument
            {
                Id = entity.Id,
                Ticker = entity.Ticker,
                Name = entity.Name,
                Type = entity.Type,
                IsSelected = entity.IsSelected,
                InPortfolio = inPortfolio,
                ManualCoefficient = entity.ManualCoefficient,
                Sector = entity.Sector
            };

            return model;
        }

        public async Task<List<Instrument>?> GetInstrumentsAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entities = await context.InstrumentEntities.ToListAsync();

            if (entities is null)
                return null;

            var models = new List<Instrument>();

            foreach (var entity in entities)
            {
                bool inPortfolio =
                    await context.BondLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker) ||
                    await context.ShareLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker) ||
                    await context.SevenEtfLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker) ||
                    await context.ThreeEtfLifePositionEntities.AnyAsync(x => x.Ticker == entity.Ticker);

                models.Add(new Instrument
                {
                    Id = entity.Id,
                    Ticker = entity.Ticker,
                    Name = entity.Name,
                    Type = entity.Type,
                    IsSelected = entity.IsSelected,
                    InPortfolio = inPortfolio,
                    ManualCoefficient = entity.ManualCoefficient,
                    Sector = entity.Sector
                });
            }

            return models;
        }
    }
}
