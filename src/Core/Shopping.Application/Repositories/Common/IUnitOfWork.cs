using Shopping.Application.Repositories.Cart;
using Shopping.Application.Repositories.Category;
using Shopping.Application.Repositories.Product;

namespace Shopping.Application.Repositories.Common;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    ICategoryRepository CategoryRepository { get; }
    IProductRepository ProductRepository { get; }
    ICartRepository CartRepository { get; }
    
    Task CommitAsync(CancellationToken cancellationToken = default);
}