using Shopping.Domain.Entities.Cart;

namespace Shopping.Application.Contracts;

public interface IReadDbContext
{
    IQueryable<CartEntity> Carts { get; }
}