using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Repositories
{
    public class ParameterRepository(
        IDbContextFactory<FinMarketContext> contextFactory)
        : IParameterRepository
    {
        public async Task<string?> GetParameterValueAsync(string name)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            var entity = await context.ParameterEntities.FirstOrDefaultAsync(x => x.Name == name);

            if (entity is null)
                return null;

            return entity.Value;
        }

        public async Task SetParameterValueAsync(string name, string value)
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            await context.ParameterEntities
                .Where(x => x.Name == name)
                .ExecuteUpdateAsync(x => x
                        .SetProperty(entity => entity.Value, value));

            await context.SaveChangesAsync();
        }
    }
}
