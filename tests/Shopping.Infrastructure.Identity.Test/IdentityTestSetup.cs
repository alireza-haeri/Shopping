using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shopping.Application.Extensions;
using Shopping.Infrastructure.Identity.Extensions;
using Shopping.Infrastructure.Persistence;
using Shopping.Infrastructure.Persistence.Extensions;
using Testcontainers.MsSql;
using IdentityServiceCollectionExtensions =
    Shopping.Infrastructure.Identity.Extensions.IdentityServiceCollectionExtensions;

namespace Shopping.Infrastructure.Identity.Test;

public class IdentityTestSetup : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        .Build();

    public IServiceProvider ServiceProvider = null!;

    public async Task InitializeAsync()
    {
        try
        {
            await _msSqlContainer.StartAsync();
        }
        catch (Exception e)
        {
            var logs = await _msSqlContainer.GetLogsAsync();
            Console.WriteLine($"Sql Container Error: {logs}");
            throw;
        }

        var dbOptionsBuilder = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseSqlServer(_msSqlContainer.GetConnectionString());

        var db = new ShoppingDbContext(dbOptionsBuilder.Options);
        await db.Database.MigrateAsync();

        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddApplicationAutoMapper()
            .AddApplicationMediatorServices()
            .RegisterApplicationValidator()
            .AddPersistenceDbContext(new AddPersistenceDbContextModel(_msSqlContainer.GetConnectionString()))
            .AddIdentityServices(new AddIdentityServicesModel(
                new AddIdentityServicesJwtModel(
                    "Test-Test-Test-Test-Test-SignIn-Key_Test",
                    "16CharEncryptKey",
                    "TestAudience",
                    "TestIssuer", 60)))
            .AddLogging(logging => logging.AddConsole());

        ServiceProvider = serviceCollection.BuildServiceProvider(false);
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
        await _msSqlContainer.DisposeAsync();
    }
}