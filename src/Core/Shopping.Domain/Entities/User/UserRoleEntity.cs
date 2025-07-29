using Microsoft.AspNetCore.Identity;
using Shopping.Domain.Common;

namespace Shopping.Domain.Entities.User;

public sealed class UserRoleEntity : IdentityUserRole<Guid>,IEntity
{
    public UserEntity User { get; set; }
    public RoleEntity Role { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}