namespace Shopping.Application.Features.Cart.Queries;

public record GetCartDetailsQueryResult(
    Guid CartId,
    IReadOnlyList<GetCartDetailsQueryResult.CartItem> Items
)
{
    public record CartItem(
        Guid CartItemId,
        Guid ProductId,
        string ProductTitle,
        string? ProductImage,
        int Quantity,
        decimal UnitPrice
    );

    public decimal GrandTotal => Items.Sum(item => item.UnitPrice * item.Quantity);
    public int TotalItems => Items.Sum(item => item.Quantity);
}