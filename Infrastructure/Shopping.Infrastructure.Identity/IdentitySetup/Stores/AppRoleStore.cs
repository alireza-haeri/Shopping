using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shopping.Domain.Entities.User;
using Shopping.Infrastructure.Persistence;

namespace Shopping.Infrastructure.Identity.IdentitySetup.Stores;

internal class AppRoleStore(ShoppingDbContext context, IdentityErrorDescriber? describer = null)
    : RoleStore<RoleEntity, ShoppingDbContext, Guid>(context, describer);