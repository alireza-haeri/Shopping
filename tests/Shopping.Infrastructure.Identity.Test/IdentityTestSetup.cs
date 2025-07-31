using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shopping.Application.Extensions;
using Shopping.Application.Repositories.Common;
using Shopping.Infrastructure.Identity.Extensions;
using Shopping.Infrastructure.Persistence;
using Shopping.Infrastructure.Persistence.Repositories.Common; 
using Testcontainers.MsSql;

namespace Shopping.Infrastructure.Identity.Test;

public class IdentityTestSetup : IAsyncLifetime
{
    public const string TestSignInKey = "ThisIsAValidTestSignInKeyForTests123";
    public const string TestEncryptionKey = "MySecret16ByteKy"; 
    
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        .Build();

    public IServiceProvider ServiceProvider { get; private set; } = null!;
    public ShoppingDbContext DbContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        var connectionString = _msSqlContainer.GetConnectionString();
        var dbContextOptions = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseSqlServer(connectionString).Options;

        DbContext = new ShoppingDbContext(dbContextOptions);
        await DbContext.Database.MigrateAsync();

        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddApplicationAutoMapper()
            .AddApplicationMediatorServices()
            .RegisterApplicationValidator()
            .AddSingleton(DbContext)
            .AddScoped<IUnitOfWork, UnitOfWork>() 
            .AddIdentityServices(new AddIdentityServicesModel(
                new AddIdentityServicesJwtModel(
                    TestSignInKey,
                    TestEncryptionKey,
                    "TestAudience",
                    "TestIssuer", 60)))
            .AddLogging(logging => logging.AddConsole());

        ServiceProvider = serviceCollection.BuildServiceProvider(false);
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }
}