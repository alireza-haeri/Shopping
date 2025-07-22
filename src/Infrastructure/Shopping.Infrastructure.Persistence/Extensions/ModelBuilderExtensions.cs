using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Shopping.Infrastructure.Persistence.Extensions;

internal static class ModelBuilderExtensions
{
    public static void RegisterEntities<TEntityType>(this ModelBuilder builder,params Assembly[] assemblies)
    {
        var entityTypes = assemblies
            .SelectMany(c => c.ExportedTypes)
            .Where(c => c is { IsClass: true, IsAbstract: false, IsPublic: true } &&
                        typeof(TEntityType).IsAssignableFrom(c));

        foreach (var entityType in entityTypes)
        {
            builder.Entity(entityType);
        }
    }


    public static void ApplyRestrictDeleteBehaviour(this ModelBuilder modelBuilder)
    {
        var cascadeForeignKeys =
            modelBuilder.Model.GetEntityTypes()
                .SelectMany(c => c.GetForeignKeys())
                .Where(c => !c.IsOwnership && c.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var cascadeForeignKey in cascadeForeignKeys)
        {
            cascadeForeignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}