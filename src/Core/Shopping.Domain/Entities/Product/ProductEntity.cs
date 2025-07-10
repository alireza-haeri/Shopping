using Ardalis.GuardClauses;
using Shopping.Domain.Common;
using Shopping.Domain.Common.ValueObjects;

namespace Shopping.Domain.Entities.Product;

public sealed class ProductEntity : BaseEntity<Guid>
{
    private ProductEntity()
    {
    }

    private readonly List<ImageValueObject> _images = [];
    private readonly List<LogValueObject> _changeLogs = [];
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public ProductState State { get; set; }
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }

    public IReadOnlyList<ImageValueObject> Images => _images.AsReadOnly();
    public IReadOnlyList<LogValueObject> ChangeLogs => _changeLogs.AsReadOnly();

    public static ProductEntity Create(Guid? id, string title, string description, decimal price, int quantity,
        ProductState state, Guid? userId, Guid? categoryId)
    {
        Guard.Against.NullOrEmpty(title, message: "Invalid Title");
        Guard.Against.NullOrEmpty(id, message: "Invalid Id");
        Guard.Against.NullOrEmpty(userId, message: "Invalid User Id");
        Guard.Against.NullOrEmpty(categoryId, message: "Invalid Category Id");
        Guard.Against.NegativeOrZero(price, message: "Invalid Price");
        Guard.Against.Negative(quantity, message: "Invalid Quantity");

        var product = new ProductEntity()
        {
            Id = id.Value,
            Title = title,
            Description = description,
            Price = price,
            Quantity = quantity,
            State = state,
            UserId = userId.Value,
            CategoryId = categoryId.Value
        };

        product._changeLogs.Add(LogValueObject.Log("Product Created"));

        return product;
    }

    public static ProductEntity Create(string title, string description, decimal price, int quantity,
        ProductState state, Guid? userId, Guid? categoryId)
        => Create(Guid.NewGuid(), title, description, price, quantity, state, userId, categoryId);

    public DomainResult ChangeState(ProductState state, string additionalDescription = null)
    {
        State = state;
        _changeLogs.Add(LogValueObject.Log("Product State Changed", additionalDescription));

        return DomainResult.Success;
    }

    public enum ProductState
    {
        Hidden = 0,
        ShowOnly = 1,
        Active = 2
    }

    public void Edit(string title, string description, int price,int quantity, Guid? categoryId)
    {
        Guard.Against.NullOrEmpty(title, message: "Invalid Title");
        Guard.Against.NullOrEmpty(categoryId, message: "Invalid Category Id");
        Guard.Against.NegativeOrZero(price, message: "Invalid Price");
        Guard.Against.Negative(quantity, message: "Invalid Quantity");
        
        Title = title;
        Description = description;
        Price = price;
        CategoryId = categoryId.Value;
        
        _changeLogs.Add(LogValueObject.Log("Product Edited"));
    }
}