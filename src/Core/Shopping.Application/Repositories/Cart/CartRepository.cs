using Shopping.Domain.Entities.Cart;

namespace Shopping.Application.Repositories.Cart;

public interface ICartRepository
{
    Task<CartEntity?> GetCartByUserIdWithTrackingAsync(Guid userId,CancellationToken cancellationToken = default);
    Task CreateCartAsync(CartEntity cart, CancellationToken cancellationToken = default);
}