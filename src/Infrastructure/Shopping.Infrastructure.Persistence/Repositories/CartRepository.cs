using Microsoft.EntityFrameworkCore;
using Shopping.Application.Repositories.Cart;
using Shopping.Domain.Entities.Cart;
using Shopping.Infrastructure.Persistence.Repositories.Common;

namespace Shopping.Infrastructure.Persistence.Repositories;

internal class CartRepository(ShoppingDbContext db) : BaseRepository<CartEntity>(db), ICartRepository
{
    public async Task<CartEntity?> GetCartByUserIdWithTrackingAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await TableNoTracking.FirstOrDefaultAsync(c=>c.UserId.Equals(userId), cancellationToken);
    }

    public async Task CreateCartAsync(CartEntity cart, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(cart, cancellationToken);
    }
}