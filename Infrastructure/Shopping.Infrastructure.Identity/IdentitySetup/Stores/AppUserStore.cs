using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shopping.Domain.Entities.User;
using Shopping.Infrastructure.Persistence;

namespace Shopping.Infrastructure.Identity.IdentitySetup.Stores;

internal class AppUserStore(ShoppingDbContext context, IdentityErrorDescriber? describer = null)
    : UserStore<UserEntity, RoleEntity, ShoppingDbContext, Guid, UserClaimEntity, UserRoleEntity, UserLoginEntity,
        UserTokenEntity, RoleClaimEntity>(context, describer);