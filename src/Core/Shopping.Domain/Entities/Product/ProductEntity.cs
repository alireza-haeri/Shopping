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
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }

    public IReadOnlyList<ImageValueObject> Images => _images;

    public static ProductEntity Create(Guid? id, string title, string description, decimal price, Guid? userId,
        Guid? categoryId)
    {
        ArgumentNullException.ThrowIfNull(title);

        if (price <= 0)
            throw new InvalidOperationException("Price must be greater than zero");

        if (id == Guid.Empty || id == null)
            throw new InvalidOperationException("Id Must Have a Value");

        if (userId == Guid.Empty || userId == null)
            throw new InvalidOperationException("User Id Must Have a Value");

        if (categoryId == Guid.Empty || categoryId == null)
            throw new InvalidOperationException("Category Id Must Have a Value");

        return new ProductEntity()
        {
            Id = id.Value,
            Title = title,
            Description = description,
            Price = price,
            UserId = userId.Value,
            CategoryId = categoryId.Value
        };
    }

    public static ProductEntity Create(string title, string description, decimal price, Guid? userId, Guid? categoryId)
    {
        ArgumentNullException.ThrowIfNull(title);

        if (price <= 0)
            throw new InvalidOperationException("Price must be greater than zero");

        if (userId == Guid.Empty || userId == null)
            throw new InvalidOperationException("User Id Must Have a Value");

        if (categoryId == Guid.Empty || categoryId == null)
            throw new InvalidOperationException("Category Id Must Have a Value");

        return new ProductEntity()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Price = price,
            UserId = userId.Value,
            CategoryId = categoryId.Value
        };
    }
}