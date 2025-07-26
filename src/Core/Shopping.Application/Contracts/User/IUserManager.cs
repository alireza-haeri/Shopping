using Microsoft.AspNetCore.Identity;
using Shopping.Domain.Entities.User;

namespace Shopping.Application.Contracts.User;

public interface IUserManager
{
    Task<IdentityResult> PasswordCreateAsync(UserEntity user,string password, CancellationToken cancellationToken);
    Task<UserEntity?> FindByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IdentityResult> ValidatePasswordAsync(UserEntity user, string password, CancellationToken cancellationToken);
}