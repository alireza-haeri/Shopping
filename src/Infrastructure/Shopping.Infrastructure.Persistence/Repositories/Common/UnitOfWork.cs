using Shopping.Application.Repositories.Category;
using Shopping.Application.Repositories.Common;
using Shopping.Application.Repositories.Product;

namespace Shopping.Infrastructure.Persistence.Repositories.Common;

public class UnitOfWork(ShoppingDbContext db) : IUnitOfWork
{
    public ICategoryRepository CategoryRepository { get; } = new CategoryRepository(db);
    public IProductRepository ProductRepository { get; } = new ProductRepository(db);
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await db.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await db.DisposeAsync();
    }
}