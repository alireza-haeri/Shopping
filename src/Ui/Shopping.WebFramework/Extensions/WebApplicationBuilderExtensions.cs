using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Shopping.Application.Extensions;
using Shopping.Domain.Entities.User;
using Shopping.Infrastructure.CrossCutting.FileStorageService.Extensions;
using Shopping.Infrastructure.CrossCutting.Logging;
using Shopping.Infrastructure.Identity.Extensions;
using Shopping.Infrastructure.Persistence.Extensions;
using Shopping.WebFramework.Models;

namespace Shopping.WebFramework.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddVersioning(this WebApplicationBuilder builder)
    {
        var currentVersion = builder.Configuration
            .GetSection("Swagger")
            .GetSection("CurrentVersion")
            .Get<double?>() ?? 1.0;

        builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(currentVersion);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        return builder;
    }

    public static WebApplicationBuilder ConfigureAuthenticationAndAuthorization(this WebApplicationBuilder builder)
    {
        var signKey = builder.Configuration.GetSection("JwtConfiguration")["SignInKey"]!;
        var encryptionKey = builder.Configuration.GetSection("JwtConfiguration")["EncryptionKey"]!;
        var issuer = builder.Configuration.GetSection("JwtConfiguration")["Issuer"]!;
        var audience = builder.Configuration.GetSection("JwtConfiguration")["Audience"]!;


        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var validationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                RequireSignedTokens = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey)),

                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey))
            };

            options.TokenValidationParameters = validationParameters;

            options.Events = new JwtBearerEvents()
            {
                OnTokenValidated = async context =>
                {
                    var signInManager =
                        context.HttpContext.RequestServices.GetRequiredService<SignInManager<UserEntity>>();

                    var claimsIdentity = context.Principal!.Identity as ClaimsIdentity;
                    if (claimsIdentity!.Claims?.Any() != true)
                        context.Fail("This token has no claims.");

                    var securityStamp =
                        claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                    if (string.IsNullOrEmpty(securityStamp))
                        context.Fail("This token has no secuirty stamp");

                    var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                    if (validatedUser is null)
                        context.Fail("Token secuirty stamp is not valid.");
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    await context.Response.WriteAsJsonAsync(new ApiResult(false, "Forbidden",
                        ApiResultStatusCode.Forbidden));
                }
            };
        });


        return builder;
    }

    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        var loggingConfiguration = builder.Configuration
            .GetSection("Logging")
            .Get<SerilogConfiguration>();

        if (loggingConfiguration is null)
            throw new ArgumentNullException(nameof(SerilogConfiguration));

        builder.Host.UseSerilog(LoggingConfiguration.ConfigureLogger(loggingConfiguration!));

        return builder;
    }

    public static WebApplicationBuilder AddIdentityServices(this WebApplicationBuilder builder)
    {
        var identityConfiguration = builder.Configuration
            .GetSection("IdentityConfiguration")
            .Get<AddIdentityServicesModel>();

        if (identityConfiguration is null)
            throw new ArgumentNullException(nameof(AddIdentityServicesModel));

        builder.Services.AddIdentityServices(identityConfiguration!);

        return builder;
    }

    public static WebApplicationBuilder AddFileStorageServices(this WebApplicationBuilder builder)
    {
        var fileStorageConfiguration = builder.Configuration
            .GetSection("FileStorageConfiguration")
            .Get<AddFileStorageServicesModel>();

        if (fileStorageConfiguration is null)
            throw new ArgumentNullException(nameof(AddIdentityServicesModel));

        builder.Services.AddFileStorageServices(fileStorageConfiguration!);

        return builder;
    }

    public static WebApplicationBuilder AddPersistenceDbContext(this WebApplicationBuilder builder)
    {
        var persistenceConfiguration = builder.Configuration
            .GetSection("AddPersistenceDbContextModel")
            .Get<AddPersistenceDbContextModel>();

        if (persistenceConfiguration is null)
            throw new ArgumentNullException(nameof(AddIdentityServicesModel));

        builder.Services.AddPersistenceDbContext(persistenceConfiguration!);

        return builder;
    }

    public static WebApplicationBuilder AddApplicationAutoMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationAutoMapper();

        return builder;
    }

    public static WebApplicationBuilder AddApplicationMediatorServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationMediatorServices();

        return builder;
    }

    public static WebApplicationBuilder RegisterApplicationValidator(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterApplicationValidator();

        return builder;
    }
}