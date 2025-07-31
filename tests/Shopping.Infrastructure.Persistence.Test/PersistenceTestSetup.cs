using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Application.Extensions;
using Shopping.Application.Repositories.Common;
using Shopping.Infrastructure.Persistence.Repositories.Common;
using Testcontainers.MsSql;

namespace Shopping.Infrastructure.Persistence.Test;

public class PersistenceTestSetup : IAsyncLifetime
{
    public UnitOfWork UnitOfWork { get; private set; } = null!;
    public ShoppingDbContext ShoppingDbContext { get; private set; } = null!;
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        //.WithDatabase("ShoppingTestDb")
        .Build();

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        var connectionString = _msSqlContainer.GetConnectionString();
        var dbContextOptions = new DbContextOptionsBuilder<ShoppingDbContext>()
            .UseSqlServer(connectionString).Options;

        ShoppingDbContext = new ShoppingDbContext(dbContextOptions);
        await ShoppingDbContext.Database.MigrateAsync();
        UnitOfWork = new UnitOfWork(ShoppingDbContext);

        var services = new ServiceCollection();
        // ثبت سرویس‌های مورد نیاز برای MediatR و سایر وابستگی‌ها
        services.AddApplicationAutoMapper();
        services.AddApplicationMediatorServices();
        services.RegisterApplicationValidator();
        // ثبت DbContext به صورت Singleton برای طول عمر تست
        services.AddSingleton(ShoppingDbContext);
        services.AddSingleton<IUnitOfWork>(UnitOfWork);

        ServiceProvider = services.BuildServiceProvider();
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
    }
}