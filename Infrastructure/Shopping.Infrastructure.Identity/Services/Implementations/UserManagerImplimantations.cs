using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping.Application.Contracts.User;
using Shopping.Domain.Entities.User;

namespace Shopping.Infrastructure.Identity.Services.Implementations;

internal class UserManagerImplementations(UserManager<UserEntity> userManager,SignInManager<UserEntity> signInManager) : IUserManager
{
    public async Task<IdentityResult> PasswordCreateAsync(UserEntity user, string password,
        CancellationToken cancellationToken)
    {
        return await userManager.CreateAsync(user, password);
    }

    public async Task<UserEntity?> FindByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await userManager.FindByNameAsync(userName);
    }

    public async Task<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await userManager.FindByIdAsync(id.ToString());
    }

    public async Task<IdentityResult> ValidatePasswordAsync(UserEntity user, string password,
        CancellationToken cancellationToken)
    {
        var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, password, true);
        if(checkPasswordResult.Succeeded)
            return IdentityResult.Success;
        
        return IdentityResult.Failed(new IdentityError(){Code = "InvalidPassword",Description = "Password is incorrect"});  
    }

    public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await userManager.Users.AnyAsync(u => userId.Equals(u.Id),cancellationToken);
    }
}