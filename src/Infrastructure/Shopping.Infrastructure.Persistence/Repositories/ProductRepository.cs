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

    public async Task<ProductEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await TableNoTracking.FirstOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
    }

    public async Task<List<ProductEntity>> GetProductsAsync(string title, int currentPage, int pageCount,
        Guid? categoryId,
        CancellationToken cancellationToken)
    {
        IQueryable<ProductEntity> productsQuery = TableNoTracking;

        if (!string.IsNullOrWhiteSpace(title))
        {
            productsQuery = productsQuery.Where(p => p.Title.Contains(title));
        }

        if (categoryId.HasValue)
        {
            productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
        }
    
        productsQuery = productsQuery.OrderByDescending(p => p.CreatedDate);

        if (currentPage <= 0)
            currentPage = 1;
    
        if (pageCount <= 0)
            pageCount = 10;
        
        var result = await productsQuery
            .Skip((currentPage - 1) * pageCount)
            .Take(pageCount)
            .ToListAsync(cancellationToken);

        return result;
    }
    public async Task DeleteAsync(ProductEntity product, CancellationToken cancellationToken)
    {
        await base.DeleteAsync(p=>product.Id.Equals(p.Id), cancellationToken);
    }
}