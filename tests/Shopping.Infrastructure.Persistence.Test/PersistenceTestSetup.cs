using Microsoft.EntityFrameworkCore;
using Shopping.Infrastructure.Persistence.Repositories.Common;
using Testcontainers.MsSql;

namespace Shopping.Infrastructure.Persistence.Test;

public class PersistenceTestSetup : IAsyncLifetime
{
    public UnitOfWork UnitOfWork { get; set; }
    private readonly MsSqlContainer _msSqlConfiguration = new MsSqlBuilder().Build();
    
    public async Task InitializeAsync()
    {
        await _msSqlConfiguration.StartAsync();
        
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseSqlServer(_msSqlConfiguration.GetConnectionString());
        
        var dbContext = new ShoppingDbContext(dbContextOptionsBuilder.Options);

        await dbContext.Database.MigrateAsync();
        
        UnitOfWork = new UnitOfWork(dbContext);
    }

    public async Task DisposeAsync()
    {
        await _msSqlConfiguration.StopAsync();
        await UnitOfWork.DisposeAsync(); 
    }
}