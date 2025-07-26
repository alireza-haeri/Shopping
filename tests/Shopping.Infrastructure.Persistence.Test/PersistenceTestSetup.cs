using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Application.Extensions;
using Shopping.Infrastructure.Persistence.Extensions;
using Shopping.Infrastructure.Persistence.Repositories.Common;
using Testcontainers.MsSql;

namespace Shopping.Infrastructure.Persistence.Test;

public class PersistenceTestSetup : IAsyncLifetime
{
    public UnitOfWork UnitOfWork { get; private set; } = null!;
    private readonly MsSqlContainer _msSqlConfiguration = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest") 
        .Build();

    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        try
        {
            await _msSqlConfiguration.StartAsync();
        }
        catch (Exception e)
        {
            var logs = await _msSqlConfiguration.GetLogsAsync();
            Console.WriteLine($"Container Logs: {logs}");
        }

        var dbContextOptionsBuilder = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseSqlServer(_msSqlConfiguration.GetConnectionString());

        var dbContext = new ShoppingDbContext(dbContextOptionsBuilder.Options);

        await dbContext.Database.MigrateAsync();

        UnitOfWork = new UnitOfWork(dbContext);

        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddApplicationAutoMapper()
            .AddApplicationMediatorServices()
            .RegisterApplicationValidator()
            .AddPersistenceDbContext(new AddPersistenceDbContextModel(_msSqlConfiguration.GetConnectionString()));
        
        ServiceProvider = serviceCollection.BuildServiceProvider(false);
    }

    public async Task DisposeAsync()
    {
        await _msSqlConfiguration.StopAsync();
        await UnitOfWork.DisposeAsync(); 
    }
}