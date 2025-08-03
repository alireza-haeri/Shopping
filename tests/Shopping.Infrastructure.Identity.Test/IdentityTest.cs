using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bogus;
using FluentAssertions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shopping.Application.Features.User.Commands.Register;
using Shopping.Application.Features.User.Queries.PasswordLogin;
using Shopping.Infrastructure.Persistence;

namespace Shopping.Infrastructure.Identity.Test;

public abstract class IdentityTestBase(IdentityTestSetup setup) : IClassFixture<IdentityTestSetup>, IAsyncLifetime
{
    protected readonly ISender Sender = setup.ServiceProvider.GetRequiredService<ISender>();
    protected readonly Faker Faker = new("fa");
    private readonly ShoppingDbContext _dbContext = setup.DbContext;
    protected readonly IdentityTestSetup Setup = setup;

    private async Task ResetDatabaseAsync()
    {
        // پاک کردن جداول برای ایزوله‌سازی هر تست
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [product].[Products]");
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [product].[Categories]");
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [user].[Users]");
    }
    
    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}


public class IdentityIntegrationTests
{
    public class UserRegistrationTests : IdentityTestBase
    {
        public UserRegistrationTests(IdentityTestSetup setup) : base(setup) { }

        [Fact]
        public async Task RegisterUser_WithValidData_ShouldSucceed()
        {
            // Arrange
            var password = Faker.Internet.Password(10);
            var command = new RegisterUserCommand(
                Faker.Person.FirstName,
                Faker.Person.LastName,
                Faker.Person.UserName,
                Faker.Person.Email,
                "09123456789",
                password,
                password);

            // Act
            var result = await Sender.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue("because user registration with valid data should succeed");
        }
    }

    public class UserLoginAndTokenTests : IdentityTestBase
    {
        public UserLoginAndTokenTests(IdentityTestSetup setup) : base(setup) { }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnAccessToken()
        {
            // Arrange
            var password = Faker.Internet.Password(10);
            var userEmail = Faker.Person.Email;
            var registerCommand = new RegisterUserCommand("Test", "User", Faker.Person.UserName, userEmail, "09123456789", password, password);
            await Sender.Send(registerCommand);

            var loginQuery = new UserPasswordLoginQuery(userEmail, password);

            // Act
            var result = await Sender.Send(loginQuery);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GeneratedToken_ShouldBeValidAndContainCorrectClaims()
        {
            // Arrange
            var password = Faker.Internet.Password(10);
            var userEmail = Faker.Person.Email;
            var userName = Faker.Person.UserName;
            
            var registerResult = await Sender.Send(new RegisterUserCommand("Test", "User", userName, userEmail, "09123456789", password, password));
            var loginResult = await Sender.Send(new UserPasswordLoginQuery(userName, password));
            var token = loginResult.Result!.AccessToken;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false, 
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(IdentityTestSetup.TestSignInKey)),
                TokenDecryptionKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(IdentityTestSetup.TestEncryptionKey))
            };

            // Act
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);

            // Assert
            principal.IsValid.Should().BeTrue();
            principal.ClaimsIdentity.IsAuthenticated.Should().BeTrue();
            
            // بررسی یک Claim خاص برای اطمینان از صحت اطلاعات کاربر در توکن
            principal.ClaimsIdentity.Claims
                .Should().ContainSingle(c => c.Type == ClaimTypes.Email && c.Value == userEmail);
        }
    }
}