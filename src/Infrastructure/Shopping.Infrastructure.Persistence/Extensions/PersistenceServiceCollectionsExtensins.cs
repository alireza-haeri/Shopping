using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Application.Repositories.Common;
using Shopping.Infrastructure.Persistence.Repositories.Common;

namespace Shopping.Infrastructure.Persistence.Extensions;

public static class PersistenceServiceCollectionsExtensins
{
    public static IServiceCollection AddPersistenceDbContext(this IServiceCollection services,
        AddPersistenceDbContextModel model)
    {
        services.AddDbContext<ShoppingDbContext>(options =>
        {
            options.UseSqlServer(model.ConnectionString
                , builder =>
                {
                    builder.EnableRetryOnFailure();
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

public record AddPersistenceDbContextModel(string ConnectionString);