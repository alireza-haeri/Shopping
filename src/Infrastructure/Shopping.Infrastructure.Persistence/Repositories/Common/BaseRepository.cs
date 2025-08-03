using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Shopping.Domain.Common;

namespace Shopping.Infrastructure.Persistence.Repositories.Common;

internal abstract class BaseRepository<TEntity>(ShoppingDbContext db)
    where TEntity : class, IEntity
{
    
    private readonly DbSet<TEntity> _entities = db.Set<TEntity>();

    private protected IQueryable<TEntity> Table => _entities;

    private protected IQueryable<TEntity> TableNoTracking => _entities.AsNoTracking();

    protected virtual async ValueTask AddAsync(TEntity entity,CancellationToken cancellationToken=default)
        =>await _entities.AddAsync(entity, cancellationToken);

    protected virtual async Task<List<TEntity>> ListAllAsync(CancellationToken cancellationToken)
        => await _entities.ToListAsync(cancellationToken);

    protected virtual async Task UpdateAsync(Expression<Func<TEntity, bool>> whereClause
        , Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateClause)
        => await _entities.Where(whereClause).ExecuteUpdateAsync(updateClause);

    protected virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> deleteClause,
        CancellationToken cancellationToken)
        => await _entities.Where(deleteClause).ExecuteDeleteAsync(cancellationToken: cancellationToken);
}