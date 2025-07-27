using FluentAssertions;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
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
}