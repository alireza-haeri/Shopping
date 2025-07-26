using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Repositories.Category;

public interface ICategoryRepository
{
    Task CreateAsync(CategoryEntity category,CancellationToken cancellationToken = default);
    Task<CategoryEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CategoryEntity?> GetByIdWithTrackingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}