using Shopping.Application.Repositories.Category;

namespace Shopping.Application.Repositories.Common;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    ICategoryRepository CategoryRepository { get; }
    
    Task CommitAsync(CancellationToken cancellationToken = default);
}