using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Repositories.Product;

public interface IProductRepository
{
    Task CreateAsync(ProductEntity product,CancellationToken cancellationToken);
    Task<ProductEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ProductEntity?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<ProductEntity>> GetProductsAsync(string title,int currentPage,int pageCount,Guid? categoryId,CancellationToken cancellationToken);
}