using Microsoft.EntityFrameworkCore;
using Shopping.Domain.Common;
using Shopping.Infrastructure.Persistence.Extensions;

namespace Shopping.Infrastructure.Persistence;

public class ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RegisterEntities<IEntity>(typeof(IEntity).Assembly);
        modelBuilder.ApplyRestrictDeleteBehaviour();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShoppingDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplyEntityChangeDates();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyEntityChangeDates();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        ApplyEntityChangeDates();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        ApplyEntityChangeDates();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyEntityChangeDates()
    {
        var entities = base.ChangeTracker
            .Entries()
            .Where(c => c is { Entity: IEntity, State: EntityState.Added } or { Entity: IEntity, State: EntityState.Modified });

        foreach (var entity in entities)
        {
            if(entity.State==EntityState.Added)
                ((IEntity)entity.Entity).CreatedDate=DateTime.Now;
            
            else if(entity.State==EntityState.Modified)
                ((IEntity)entity.Entity).ModifiedDate=DateTime.Now;
        }
    }
}