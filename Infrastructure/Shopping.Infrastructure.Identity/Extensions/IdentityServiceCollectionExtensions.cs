using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Application.Contracts.User;
using Shopping.Domain.Entities.User;
using Shopping.Infrastructure.Identity.IdentitySetup.Factories;
using Shopping.Infrastructure.Identity.IdentitySetup.Stores;
using Shopping.Infrastructure.Identity.Services.Implementations;
using Shopping.Infrastructure.Identity.Services.Model;
using Shopping.Infrastructure.Persistence;

namespace Shopping.Infrastructure.Identity.Extensions;

public static class IdentityServiceCollectionExtensions
{
    
    
    public static IServiceCollection AddIdentityServices(this IServiceCollection services,AddIdentityServicesModel model)
    {
        services.AddScoped<IUserClaimsPrincipalFactory<UserEntity>, AppUserClaimPrincipalFactory>();
        services.AddScoped<IRoleStore<RoleEntity>, AppRoleStore>();
        services.AddScoped<IUserStore<UserEntity>, AppUserStore>();

        services.AddIdentity<UserEntity, RoleEntity>(options =>
            {
                options.Stores.ProtectPersonalData = false;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireUppercase = false;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = false;
                options.User.RequireUniqueEmail = false;
            })
            .AddRoleStore<AppRoleStore>()
            .AddUserStore<AppUserStore>()
            .AddClaimsPrincipalFactory<AppUserClaimPrincipalFactory>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ShoppingDbContext>();

        services.Configure<JwtConfiguration>(config =>
        {
            var jwtModel = model.JwtModel;
            
            config.SignInKey = jwtModel.SignInKey;
            config.Audience = jwtModel.Audience;
            config.Issuer = jwtModel.Issuer;
            config.ExpirationMinute = jwtModel.ExpirationMinute;
            config.EncryptionKey = jwtModel.EncryptionKey;
        });

        services.AddScoped<IJwtService, JwtServiceImplementation>();
        services.AddScoped<IUserManager, UserManagerImplementations>();
        
        return services;
    }
}

public record AddIdentityServicesModel(AddIdentityServicesJwtModel JwtModel);

public record AddIdentityServicesJwtModel(string SignInKey,string EncryptionKey, string Audience, string Issuer, int ExpirationMinute);