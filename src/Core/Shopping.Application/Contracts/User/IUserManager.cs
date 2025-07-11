using Microsoft.AspNetCore.Identity;
using Shopping.Domain.Entities.User;

namespace Shopping.Application.Contracts.User;

public interface IUserManager
{
    Task<IdentityResult> CreateAsync(UserEntity user , CancellationToken cancellationToken);
    Task<UserEntity?> FindByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<IdentityResult> PasswordSignInAsync(UserEntity user, string password, bool rememberMe, CancellationToken cancellationToken);
}