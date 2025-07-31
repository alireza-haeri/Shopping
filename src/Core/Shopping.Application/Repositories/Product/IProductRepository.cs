using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Repositories.Product;

public interface IProductRepository
{
    Task CreateAsync(ProductEntity product,CancellationToken cancellationToken = default);
    Task<ProductEntity?> GetByIdAsTrackAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductEntity?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductEntity>> GetProductsAsync(string title,int currentPage,int pageCount,Guid? categoryId,CancellationToken cancellationToken = default);
    Task DeleteAsync(ProductEntity product, CancellationToken cancellationToken = default);
}