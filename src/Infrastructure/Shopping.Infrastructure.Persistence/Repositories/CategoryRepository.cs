using Microsoft.EntityFrameworkCore;
using Shopping.Application.Repositories.Category;
using Shopping.Domain.Entities.Product;
using Shopping.Infrastructure.Persistence.Repositories.Common;

namespace Shopping.Infrastructure.Persistence.Repositories;

internal class CategoryRepository(ShoppingDbContext db) : BaseRepository<CategoryEntity>(db), ICategoryRepository
{
    public async Task CreateAsync(CategoryEntity category, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(category, cancellationToken);
    }

    public async Task<CategoryEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
       return await base.TableNoTracking.FirstOrDefaultAsync(c => id.Equals(c.Id),cancellationToken);
    }

    public async Task<CategoryEntity?> GetByIdWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Table.FirstOrDefaultAsync(c => c.Id.Equals(id), cancellationToken);
    }

    public async Task<List<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await TableNoTracking.ToListAsync(cancellationToken);
    }
}