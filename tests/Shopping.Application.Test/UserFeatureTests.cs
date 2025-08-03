using Bogus;
using FluentAssertions;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shopping.Application.Common;
using Shopping.Application.Contracts.User;
using Shopping.Application.Contracts.User.Models;
using Shopping.Application.Extensions;
using Shopping.Application.Features.Common;
using Shopping.Application.Features.User.Commands.Register;
using Shopping.Application.Features.User.Queries.PasswordLogin;
using Shopping.Domain.Entities.User;
using Xunit.Abstractions;

namespace Shopping.Application.Test;

/// <summary>
/// Base class for User feature tests to handle common setup and mocking.
/// </summary>
public abstract class UserFeaturesTestBase
{
    protected readonly ITestOutputHelper TestOutputHelper;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IUserManager UserManagerMock;
    protected readonly IJwtService JwtServiceMock;
    protected readonly Faker Faker;

    protected UserFeaturesTestBase(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
        Faker = new Faker("fa");

        // Configure services and build the service provider for validators
        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterApplicationValidator();
        ServiceProvider = serviceCollection.BuildServiceProvider();

        // Setup mock objects for dependencies
        UserManagerMock = Substitute.For<IUserManager>();
        JwtServiceMock = Substitute.For<IJwtService>();
    }

    /// <summary>
    /// A helper method to run validation and then execute the handler.
    /// This simulates the MediatR validation pipeline behavior.
    /// </summary>
    protected async Task<TResponse> ValidateAndExecuteAsync<TRequest, TResponse>(
        TRequest request,
        IRequestHandler<TRequest, TResponse> handler)
        where TRequest : IRequest<TResponse>
        where TResponse : IOperationResult, new()
    {
        var validator = ServiceProvider.GetService<IValidator<TRequest>>();
        if (validator == null)
        {
            return await handler.Handle(request, CancellationToken.None);
        }

        var validationBehavior = new ValidateRequestBehavior<TRequest, TResponse>(validator);

        // The handler's Handle method is passed as the 'next' delegate.
        return await validationBehavior.Handle(request, CancellationToken.None,
            (req, token) => handler.Handle(req, token));
    }
}

public class UserFeaturesTests
{
    public class RegisterUserTests : UserFeaturesTestBase
    {
        public RegisterUserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task Handle_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var password = Faker.Internet.Password(8);
            var command = new RegisterUserCommand(
                Faker.Person.FirstName,
                Faker.Person.LastName,
                Faker.Person.UserName,
                Faker.Person.Email,
                "09123456789",
                password,
                password);

            UserManagerMock.PasswordCreateAsync(Arg.Any<UserEntity>(), command.Password, CancellationToken.None)
                .Returns(IdentityResult.Success);

            var handler = new RegisterUserCommandHandler(UserManagerMock);

            // Act
            var result = await ValidateAndExecuteAsync(command, handler);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WhenUserManagerFails_ShouldReturnFailure()
        {
            // Arrange
            var password = Faker.Internet.Password(8);
            var command = new RegisterUserCommand("FirstName", "LastName", "username", "test@test.com", "09123456789",
                password, password);

            var identityError = new IdentityError
                { Code = "DuplicateUserName", Description = "Username is already taken." };
            UserManagerMock.PasswordCreateAsync(Arg.Any<UserEntity>(), command.Password, CancellationToken.None)
                .Returns(IdentityResult.Failed(identityError));

            var handler = new RegisterUserCommandHandler(UserManagerMock);

            // Act
            var result = await ValidateAndExecuteAsync(command, handler);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessages.Should().Contain(e => e.Key == "GeneralErrors");
        }

        [Theory]
        [InlineData("", "password")] // Empty UserNameOrEmail
        [InlineData("username", "")] // Empty Password
        public async Task Handle_WithInvalidInputs_ShouldFailValidation(string userNameOrEmail, string password)
        {
            // Arrange
            var command = new UserPasswordLoginQuery(userNameOrEmail, password);
            var handler = new UserPasswordLoginQueryHandler(UserManagerMock, JwtServiceMock);

            // Act
            var result = await ValidateAndExecuteAsync(command, handler);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessages.Should().NotBeNullOrEmpty();
        }
    }

    public class LoginUserTests : UserFeaturesTestBase
    {
        public LoginUserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task Handle_WithCorrectUserName_ShouldReturnSuccessWithToken()
        {
            // Arrange
            var password = Faker.Internet.Password();
            var user = new UserEntity(Faker.Person.FirstName, Faker.Person.LastName, Faker.Person.UserName,
                Faker.Person.Email);
            var query = new UserPasswordLoginQuery(user.UserName, password);
            var token = new JwtAccessTokenModel("jwt.token.here", 3600);

            UserManagerMock.FindByUserNameAsync(query.UserNameOrEmail, CancellationToken.None).Returns(user);
            UserManagerMock.ValidatePasswordAsync(user, query.Password, CancellationToken.None)
                .Returns(IdentityResult.Success);
            JwtServiceMock.GenerateJwtTokenAsync(user, CancellationToken.None).Returns(token);

            var handler = new UserPasswordLoginQueryHandler(UserManagerMock, JwtServiceMock);

            // Act
            var result = await ValidateAndExecuteAsync(query, handler);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().Be(token);
        }

        [Fact]
        public async Task Handle_WithCorrectEmail_ShouldReturnSuccessWithToken()
        {
            // Arrange
            var password = Faker.Internet.Password();
            var user = new UserEntity(Faker.Person.FirstName, Faker.Person.LastName, Faker.Person.UserName,
                Faker.Person.Email);
            var query = new UserPasswordLoginQuery(user.Email, password);
            var token = new JwtAccessTokenModel("jwt.token.here", 3600);

            UserManagerMock.FindByEmailAsync(query.UserNameOrEmail, CancellationToken.None).Returns(user);
            UserManagerMock.ValidatePasswordAsync(user, query.Password, CancellationToken.None)
                .Returns(IdentityResult.Success);
            JwtServiceMock.GenerateJwtTokenAsync(user, CancellationToken.None).Returns(token);

            var handler = new UserPasswordLoginQueryHandler(UserManagerMock, JwtServiceMock);

            // Act
            var result = await ValidateAndExecuteAsync(query, handler);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().Be(token);
        }

        [Fact]
        public async Task Handle_WithWrongPassword_ShouldReturnFailure()
        {
            // Arrange
            var password = Faker.Internet.Password();
            var user = new UserEntity(Faker.Person.FirstName, Faker.Person.LastName, Faker.Person.UserName,
                Faker.Person.Email);
            var query = new UserPasswordLoginQuery(user.UserName, password);

            UserManagerMock.FindByUserNameAsync(query.UserNameOrEmail, CancellationToken.None).Returns(user);
            UserManagerMock.ValidatePasswordAsync(user, query.Password, CancellationToken.None)
                .Returns(IdentityResult.Failed());

            var handler = new UserPasswordLoginQueryHandler(UserManagerMock, JwtServiceMock);

            // Act
            var result = await ValidateAndExecuteAsync(query, handler);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessages.Should().Contain(e => e.Key == nameof(UserPasswordLoginQuery.Password));
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ShouldReturnNotFound()
        {
            // Arrange
            var query = new UserPasswordLoginQuery(Faker.Person.UserName, Faker.Internet.Password());

            UserManagerMock.FindByUserNameAsync(query.UserNameOrEmail, CancellationToken.None)
                .Returns(Task.FromResult<UserEntity?>(null));

            var handler = new UserPasswordLoginQueryHandler(UserManagerMock, JwtServiceMock);

            // Act
            var result = await ValidateAndExecuteAsync(query, handler);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsNotFound.Should().BeTrue();
        }
    }
}