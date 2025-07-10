using Microsoft.AspNetCore.Identity;
using Shopping.Domain.Common;

namespace Shopping.Domain.Entities.User;

public sealed class UserLoginEntity : IdentityUserLogin<Guid>,IEntity
{
    public UserEntity User { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime ModifyDate { get; set; }
}