using Ardalis.GuardClauses;
using Shopping.Domain.Common;
using Shopping.Domain.Common.ValueObjects;

namespace Shopping.Domain.Entities.Product;

public class ProductEntity : BaseEntity<Guid>
{
    private ProductEntity()
    {
    }

    private readonly List<ImageValueObject> _images = [];
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }

    public IReadOnlyList<ImageValueObject> Images => _images;

    public static ProductEntity Create(Guid? id, string title, string description, decimal price, int quantity,
        Guid? userId,
        Guid? categoryId)
    {
        Guard.Against.NullOrEmpty(title, message: "Invalid Title");
        Guard.Against.NullOrEmpty(id, message: "Invalid Id");
        Guard.Against.NullOrEmpty(userId, message: "Invalid User Id");
        Guard.Against.NullOrEmpty(categoryId, message: "Invalid Category Id");
        Guard.Against.NegativeOrZero(price, message: "Invalid Price");
        Guard.Against.Negative(quantity, message: "Invalid Quantity");

        return new ProductEntity()
        {
            Id = id.Value,
            Title = title,
            Description = description,
            Price = price,
            Quantity = quantity,
            UserId = userId.Value,
            CategoryId = categoryId.Value
        };
    }

    public static ProductEntity Create(string title, string description, decimal price, int quantity, Guid? userId,
        Guid? categoryId)
        => Create(Guid.NewGuid(), title, description, price, quantity, userId, categoryId);
}