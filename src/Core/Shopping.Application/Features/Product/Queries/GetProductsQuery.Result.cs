namespace Shopping.Application.Features.Product.Queries;

public record GetProductsQueryResult(
    Guid ProductId,
    string Title,
    decimal Price,
    int Quantity)
{
    public record ProductImageModel(string ImageName, string ImageUrl);

    public ProductImageModel ProductImage { get; set; }

};