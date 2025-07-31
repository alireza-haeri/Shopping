using Ardalis.GuardClauses;
using Shopping.Domain.Common;

namespace Shopping.Domain.Entities.Cart;

public class CartEntity : BaseEntity<Guid>
{
    private readonly List<CartItemEntity> _items = [];
    private CartEntity(){}

    public Guid UserId { get; private set; }

    public IReadOnlyList<CartItemEntity> Items => _items.AsReadOnly();

    public decimal TotalPrice => _items.Sum(i => i.UnitPrice * i.Quantity);

    public static CartEntity Create(Guid userId)
    {
        Guard.Against.Default(userId, "User ID is required.");

        return new CartEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        Guard.Against.Default(productId, "Product ID is required.");
        Guard.Against.NegativeOrZero(quantity, "Quantity must be positive.");
        Guard.Against.NegativeOrZero(unitPrice, "Unit price must be positive.");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            var newItem = CartItemEntity.Create(productId, quantity, unitPrice);
            _items.Add(newItem);
        }
    }

    public void RemoveItem(Guid cartItemId)
    {
        Guard.Against.Default(cartItemId, "Cart Item ID is required.");

        var itemToRemove = _items.FirstOrDefault(i => i.Id == cartItemId);
        if (itemToRemove != null)
        {
            _items.Remove(itemToRemove);
        }
    }

    public void UpdateItemQuantity(Guid cartItemId, int newQuantity)
    {
        Guard.Against.Default(cartItemId, "Cart Item ID is required.");
        Guard.Against.NegativeOrZero(newQuantity, "New quantity must be positive.");

        var itemToUpdate = _items.FirstOrDefault(i => i.Id == cartItemId);
        if (itemToUpdate != null)
        {
            itemToUpdate.SetQuantity(newQuantity);
        }
    }

    public void Clear()
    {
        _items.Clear();
    }

}