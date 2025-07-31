using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Shopping.Domain.Common;
using Shopping.Domain.Entities.Product;

namespace Shopping.Domain.Entities.User;

public sealed class UserEntity : IdentityUser<Guid>, IEntity
{
    private readonly List<ProductEntity> _products = [];

    public UserEntity(string firstName, string? lastName, string userName, string email)
        : base(userName)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        UserCode = Guid.NewGuid().ToString("N")[..7];
        Email = email;
    }

    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string UserCode { get; private set; }
    public IReadOnlyList<ProductEntity> Products => _products.AsReadOnly();

    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
    public ICollection<UserClaimEntity> UserClaims { get; set; } = [];
    public ICollection<UserLoginEntity> UserLogins { get; set; } = [];
    public ICollection<UserTokenEntity> UserTokens { get; set; } = [];

    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public void AddProduct(ProductEntity product)
    {
        Guard.Against.Null(product, nameof(product));
        
        _products.Add(product);
    }
}