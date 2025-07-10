using Microsoft.AspNetCore.Identity;
using Shopping.Domain.Common;

namespace Shopping.Domain.Entities.User;

public sealed class RoleClaimEntity : IdentityRoleClaim<Guid>,IEntity
{
    public RoleEntity Role { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime ModifyDate { get; set; }
}