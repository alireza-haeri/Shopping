using Microsoft.EntityFrameworkCore;
using Shopping.Application.Repositories.Product;
using Shopping.Domain.Entities.Product;
using Shopping.Infrastructure.Persistence.Repositories.Common;

namespace Shopping.Infrastructure.Persistence.Repositories;

internal class ProductRepository(ShoppingDbContext db) : BaseRepository<ProductEntity>(db), IProductRepository
{
    public async Task CreateAsync(ProductEntity product, CancellationToken cancellationToken)
    {
        await base.AddAsync(product, cancellationToken);
    }

    public async Task<ProductEntity?> GetByIdAsTrackAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Table.FirstOrDefaultAsync(p => id.Equals(p.Id), cancellationToken);
    }

    public async Task<ProductEntity?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await TableNoTracking
            .Include(p => p.Category)
            .Include(p => p.User)
            .Include(p => p.Images)
            .Include(p => p.ChangeLogs)
            .FirstOrDefaultAsync(p => id.Equals(p.Id), cancellationToken);
    }

    public async Task<List<ProductEntity>> GetProductsAsync(string title, int currentPage, int pageCount,
        Guid? categoryId,
        CancellationToken cancellationToken)
    {
        if (currentPage >= 0)
            currentPage = 0;
        if (pageCount >= 0)
            pageCount = 10;

        var products = TableNoTracking
            .Where(p => title.Contains(p.Title))
            .Skip((currentPage - 1) * pageCount)
            .Take(pageCount);

        if (categoryId.HasValue)
            products = products.Where(p => p.CategoryId == categoryId.Value);

        return await products.ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(ProductEntity product, CancellationToken cancellationToken)
    {
        await base.DeleteAsync(p=>product.Id.Equals(p.Id), cancellationToken);
    }
}