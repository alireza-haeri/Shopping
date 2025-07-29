using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shopping.Application.Features.User.Commands.Register;
using Shopping.Application.Features.User.Queries.PasswordLogin;

namespace Shopping.Infrastructure.Identity.Test;

public class IdentityTests(IdentityTestSetup setup) : IClassFixture<IdentityTestSetup>
{
    private readonly IServiceProvider _serviceProvider = setup.ServiceProvider;
    private readonly ISender _sender = setup.ServiceProvider.GetRequiredService<ISender>();

    [Fact]
    public async Task Register_User_Should_be_Success()
    {
        var result = await _sender.Send(new RegisterUserCommand(
            "FirstName1",
            null,
            "username1",
            "email1@gmail.com",
            "09123456545",
            "password123456",
            "password123456"
        ));

        result.IsSuccess.Should().Be(true);
    }

    [Fact]
    public async Task Getting_Access_Token_Should_be_Success()
    {
        await _sender.Send(new RegisterUserCommand(
            "FirstName2",
            null,
            "username2",
            "email2@gmail.com",
            "09123456542",
            "password123452",
            "password123452"
        ));

        var gettingAccessTokenResult = await _sender.Send(
            new UserPasswordLoginQuery("username2", "password123452"));

        gettingAccessTokenResult.IsSuccess.Should().Be(true);
        gettingAccessTokenResult.Result.Should().NotBeNull();
    }

    [Fact]
    public async Task Jwe_Token_Should_Have_Claims()
    {
        await _sender.Send(new RegisterUserCommand(
            "FirstName3",
            null,
            "username3",
            "email2@gmail.com",
            "09123456543",
            "password123453",
            "password123453"
        ));

        var gettingAccessTokenResult = await _sender.Send(
            new UserPasswordLoginQuery("username3", "password123453"));

        gettingAccessTokenResult.IsSuccess.Should().Be(true);
        gettingAccessTokenResult.Result.Should().NotBeNull();

        var signInKey = "Test-Test-Test-Test-Test-SignIn-Key_Test"u8.ToArray();
        var encryptionKey = "16CharEncryptKey"u8.ToArray();

        var tokenValidation = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(signInKey),
            TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var claimPrincipals =
            await tokenHandler.ValidateTokenAsync(gettingAccessTokenResult.Result.AccessToken, tokenValidation);
        
        claimPrincipals.Should().NotBeNull();
        claimPrincipals.Claims.Should().NotBeNullOrEmpty();
    }
}