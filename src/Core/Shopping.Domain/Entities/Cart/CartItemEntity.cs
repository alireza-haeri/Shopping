using Ardalis.GuardClauses;
using Shopping.Domain.Common;
using Shopping.Domain.Entities.Product;

namespace Shopping.Domain.Entities.Cart;

public class CartItemEntity : BaseEntity<Guid>
{
    private CartItemEntity() { }

    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public Guid CartId { get; private set; }

    #region NavigationProperties

    public ProductEntity Product { get; set; }
    public CartEntity Cart { get; set; }

    #endregion
    
    public static CartItemEntity Create(Guid productId, int quantity, decimal unitPrice)
    {
        Guard.Against.Default(productId, "Product ID is required.");
        Guard.Against.NegativeOrZero(quantity, "Quantity must be positive.");
        Guard.Against.NegativeOrZero(unitPrice, "Unit price must be positive.");

        return new CartItemEntity
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }

    public void IncreaseQuantity(int quantityToAdd)
    {
        Guard.Against.NegativeOrZero(quantityToAdd, "Quantity to add must be positive.");
        Quantity += quantityToAdd;
    }

    public void SetQuantity(int newQuantity)
    {
        Guard.Against.NegativeOrZero(newQuantity, "Quantity must be positive.");
        Quantity = newQuantity;
    }
}