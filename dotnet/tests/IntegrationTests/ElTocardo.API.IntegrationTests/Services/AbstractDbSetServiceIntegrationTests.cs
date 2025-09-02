using Microsoft.EntityFrameworkCore;

namespace ElTocardo.API.IntegrationTests.Services;

public abstract class AbstractDbSetServiceIntegrationTests<TEntity>(DbContext dbContext, DbSet<TEntity> dbSet)
    : IAsyncLifetime
    where TEntity : class
{
    public async Task InitializeAsync()
    {
        await dbContext.Database.EnsureCreatedAsync();
        await ClearDbSetAsync();
    }

    public async Task DisposeAsync()
    {
        await ClearDbSetAsync();
    }

    private async Task ClearDbSetAsync()
    {
        var entities = await dbSet.ToListAsync();
        dbContext.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}
