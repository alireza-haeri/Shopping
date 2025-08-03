using Microsoft.AspNetCore.Identity;
using Shopping.Domain.Common;

namespace Shopping.Domain.Entities.User;

public sealed class RoleEntity : IdentityRole<Guid>, IEntity
{
    public RoleEntity(string name, string displayName)
        : base(name)
    {
        Name = name;
        DisplayName = displayName;
    }

    public string DisplayName { get; private set; }
    public ICollection<UserRoleEntity> UserRoles { get; set; }= [];
    public ICollection<RoleClaimEntity> RoleClaims { get; set; }= [];

    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}