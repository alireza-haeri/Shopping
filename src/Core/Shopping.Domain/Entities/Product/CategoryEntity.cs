using Shopping.Domain.Common;

namespace Shopping.Domain.Entities.Product;

public sealed class CategoryEntity : BaseEntity<Guid>
{
    private readonly List<ProductEntity> _products = [];

    public CategoryEntity(string title, Guid? parentId)
    {
        Title = title;
        ParentId = parentId;
    }

    public string Title { get; private set; }
    public Guid? ParentId { get; private set; }
    public IReadOnlyList<ProductEntity> Products => _products.AsReadOnly();
}