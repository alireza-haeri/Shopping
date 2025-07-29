using Bogus;
using FluentAssertions;
using FluentValidation;
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
using Shopping.Application.Test.Extensions;
using Shopping.Domain.Entities.User;
using Xunit.Abstractions;

namespace Shopping.Application.Test;

public class UserFeatureTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IServiceProvider _serviceProvider;

    public UserFeatureTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterApplicationValidator();
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task Creating_New_User_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var password = faker.Internet.Password();
        var registerUserRequest = new RegisterUserCommand(
            faker.Person.FirstName,
            faker.Person.LastName,
            faker.Person.UserName,
            faker.Person.Email,
            "09944853827",
            password, password);

        var userManager = Substitute.For<IUserManager>();

        userManager.PasswordCreateAsync(Arg.Any<UserEntity>(), password, CancellationToken.None)
            .Returns(Task.FromResult(IdentityResult.Success));

        //Act
        var userRegisterCommandHandler = new RegisterUserCommandHandler(userManager);
        var userRegisterResult = await userRegisterCommandHandler.Handle(registerUserRequest, CancellationToken.None);

        userRegisterResult.IsSuccess.Should().Be(true);
    }

    [Fact]
    public async Task User_Register_Email_Should_Be_Valid()
    {
        //Arrange
        var faker = new Faker();
        var password = faker.Internet.Password();
        var registerUserRequest = new RegisterUserCommand(
            faker.Person.FirstName,
            faker.Person.LastName,
            faker.Person.UserName,
            string.Empty,
            "09944853827",
            password, password);

        var userManager = Substitute.For<IUserManager>();

        userManager.PasswordCreateAsync(Arg.Any<UserEntity>(), password, CancellationToken.None)
            .Returns(Task.FromResult(IdentityResult.Success));

        //Act
        var userRegisterCommandHandler = new RegisterUserCommandHandler(userManager);
        var validationBehavior =
            new ValidateRequestBehavior<RegisterUserCommand, OperationResult<bool>>(
                _serviceProvider.GetRequiredService<IValidator<RegisterUserCommand>>());
        var userRegisterResult = await validationBehavior.Handle(registerUserRequest, CancellationToken.None,
            userRegisterCommandHandler.Handle);

        userRegisterResult.IsSuccess.Should().Be(false);

        _testOutputHelper.WriteLineOperationResultErrors(userRegisterResult);
    }

    [Fact]
    public async Task User_Register_Password_And_RepeatPassword_Should_Be_Same()
    {
        //Arrange
        var faker = new Faker();
        var password = faker.Internet.Password();
        var rePassword = faker.Internet.Password();
        var registerUserRequest = new RegisterUserCommand(
            faker.Person.FirstName,
            faker.Person.LastName,
            faker.Person.UserName,
            faker.Person.Email,
            "09944853827",
            password, rePassword);

        var userManager = Substitute.For<IUserManager>();

        userManager.PasswordCreateAsync(Arg.Any<UserEntity>(), password, CancellationToken.None)
            .Returns(Task.FromResult(IdentityResult.Success));

        //Act
        var userRegisterCommandHandler = new RegisterUserCommandHandler(userManager);
        var validationBehavior =
            new ValidateRequestBehavior<RegisterUserCommand, OperationResult<bool>>
                (_serviceProvider.GetRequiredService<IValidator<RegisterUserCommand>>());
        var userRegisterResult = await validationBehavior.Handle(registerUserRequest, CancellationToken.None,
            userRegisterCommandHandler.Handle);

        //Assertion
        userRegisterResult.IsSuccess.Should().Be(false);
        _testOutputHelper.WriteLineOperationResultErrors(userRegisterResult);
    }

    [Fact]
    public async Task Login_User_With_UserName_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var password = faker.Internet.Password();
        var userPasswordLoginQuery = new UserPasswordLoginQuery(
            faker.Person.UserName, password);

        var userEntity = new UserEntity(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.UserName,
                faker.Person.Email)
            { PhoneNumber = "09944853827", };

        var accessToken = new JwtAccessTokenModel("AccessToken", 1);


        var userManager = Substitute.For<IUserManager>();
        userManager.FindByUserNameAsync(userPasswordLoginQuery.UserNameOrEmail, CancellationToken.None)
            .Returns(Task.FromResult<UserEntity?>(userEntity));
        userManager.ValidatePasswordAsync(Arg.Any<UserEntity>(), password, CancellationToken.None)
            .Returns(Task.FromResult(IdentityResult.Success));

        var jwtService = Substitute.For<IJwtService>();
        jwtService.GenerateJwtTokenAsync(Arg.Any<UserEntity>(), CancellationToken.None)
            .Returns(Task.FromResult(accessToken));

        //Act
        var userPasswordLoginQueryHandler = new UserPasswordLoginQueryHandler(userManager, jwtService);
        var userPasswordLoginResult =
            await userPasswordLoginQueryHandler.Handle(userPasswordLoginQuery, CancellationToken.None);

        //Assertion
        userPasswordLoginResult.Result.Should().NotBeNull();
        userPasswordLoginResult.IsSuccess.Should().Be(true);
    }

    [Fact]
    public async Task Login_User_With_UserName_And_Wrong_Password_Should_Be_Failure()
    {
        //Arrange
        var faker = new Faker();
        var password = faker.Internet.Password();
        var userPasswordLoginQuery = new UserPasswordLoginQuery(
            faker.Person.UserName, password);

        var userEntity = new UserEntity(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.UserName,
                faker.Person.Email)
            { PhoneNumber = "09944853827", };

        var accessToken = new JwtAccessTokenModel("AccessToken", 1);


        var userManager = Substitute.For<IUserManager>();
        userManager.FindByUserNameAsync(userPasswordLoginQuery.UserNameOrEmail, CancellationToken.None)
            .Returns(Task.FromResult<UserEntity?>(userEntity));
        userManager.ValidatePasswordAsync(Arg.Any<UserEntity>(), password, CancellationToken.None)
            .Returns(Task.FromResult(IdentityResult.Failed()));

        var jwtService = Substitute.For<IJwtService>();
        jwtService.GenerateJwtTokenAsync(Arg.Any<UserEntity>(), CancellationToken.None)
            .Returns(Task.FromResult(accessToken));

        //Act
        var userPasswordLoginQueryHandler = new UserPasswordLoginQueryHandler(userManager, jwtService);
        var userPasswordLoginResult =
            await userPasswordLoginQueryHandler.Handle(userPasswordLoginQuery, CancellationToken.None);

        //Assertion
        userPasswordLoginResult.Result.Should().BeNull();
        userPasswordLoginResult.IsSuccess.Should().Be(false);

        _testOutputHelper.WriteLineOperationResultErrors(userPasswordLoginResult);
    }

    [Fact]
    public async Task Login_User_With_Email_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var password = faker.Internet.Password();
        var userPasswordLoginQuery = new UserPasswordLoginQuery(
            faker.Person.Email, password);

        var userEntity = new UserEntity(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.UserName,
                faker.Person.Email)
            { PhoneNumber = "09944853827", };

        var accessToken = new JwtAccessTokenModel("AccessToken", 1);


        var userManager = Substitute.For<IUserManager>();
        userManager.FindByEmailAsync(userPasswordLoginQuery.UserNameOrEmail, CancellationToken.None)
            .Returns(Task.FromResult<UserEntity?>(userEntity));
        userManager.ValidatePasswordAsync(Arg.Any<UserEntity>(), password, CancellationToken.None)
            .Returns(Task.FromResult(IdentityResult.Success));

        var jwtService = Substitute.For<IJwtService>();
        jwtService.GenerateJwtTokenAsync(Arg.Any<UserEntity>(), CancellationToken.None)
            .Returns(Task.FromResult(accessToken));

        //Act
        var userPasswordLoginQueryHandler = new UserPasswordLoginQueryHandler(userManager, jwtService);
        var userPasswordLoginResult =
            await userPasswordLoginQueryHandler.Handle(userPasswordLoginQuery, CancellationToken.None);

        //Assertion
        userPasswordLoginResult.Result.Should().NotBeNull();
        userPasswordLoginResult.IsSuccess.Should().Be(true);
    }

    [Fact]
    public async Task Login_User_With_Email_And_Wrong_Password_Should_Be_Failure()
    {
        //Arrange
        var faker = new Faker();
        var password = faker.Internet.Password();
        var userPasswordLoginQuery = new UserPasswordLoginQuery(
            faker.Person.Email, password);

        var userEntity = new UserEntity(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.UserName,
                faker.Person.Email)
            { PhoneNumber = "09944853827", };

        var accessToken = new JwtAccessTokenModel("AccessToken", 1);


        var userManager = Substitute.For<IUserManager>();
        userManager.FindByEmailAsync(userPasswordLoginQuery.UserNameOrEmail, CancellationToken.None)
            .Returns(Task.FromResult<UserEntity?>(userEntity));
        userManager.ValidatePasswordAsync(Arg.Any<UserEntity>(), password, CancellationToken.None)
            .Returns(Task.FromResult(IdentityResult.Failed()));

        var jwtService = Substitute.For<IJwtService>();
        jwtService.GenerateJwtTokenAsync(Arg.Any<UserEntity>(), CancellationToken.None)
            .Returns(Task.FromResult(accessToken));

        //Act
        var userPasswordLoginQueryHandler = new UserPasswordLoginQueryHandler(userManager, jwtService);
        var userPasswordLoginResult =
            await userPasswordLoginQueryHandler.Handle(userPasswordLoginQuery, CancellationToken.None);

        //Assertion
        userPasswordLoginResult.Result.Should().BeNull();
        userPasswordLoginResult.IsSuccess.Should().Be(false);

        _testOutputHelper.WriteLineOperationResultErrors(userPasswordLoginResult);
    }

    [Fact]
    public async Task Login_User_Not_Found_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var password = faker.Internet.Password();
        var userPasswordLoginQuery = new UserPasswordLoginQuery(
            faker.Person.Email, password);

        var accessToken = new JwtAccessTokenModel("AccessToken", 1);

        var userManager = Substitute.For<IUserManager>();
        userManager.FindByEmailAsync(userPasswordLoginQuery.UserNameOrEmail, CancellationToken.None)
            .Returns(Task.FromResult<UserEntity?>(null));

        var jwtService = Substitute.For<IJwtService>();
        jwtService.GenerateJwtTokenAsync(Arg.Any<UserEntity>(), CancellationToken.None)
            .Returns(Task.FromResult(accessToken));

        //Act
        var userPasswordLoginQueryHandler = new UserPasswordLoginQueryHandler(userManager, jwtService);
        var userPasswordLoginResult =
            await userPasswordLoginQueryHandler.Handle(userPasswordLoginQuery, CancellationToken.None);

        //Assertion
        userPasswordLoginResult.Result.Should().BeNull();
        userPasswordLoginResult.IsNotFound.Should().Be(true);
        userPasswordLoginResult.IsSuccess.Should().Be(false);

        _testOutputHelper.WriteLineOperationResultErrors(userPasswordLoginResult);
    }

    [Fact]
    public async Task Login_User_Inputs_Should_Be_Valid()
    {
        //Arrange
        var faker = new Faker();
        var password = string.Empty;
        var userPasswordLoginQuery = new UserPasswordLoginQuery(
            faker.Person.Email, password);

        var userEntity = new UserEntity(
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Person.UserName,
                faker.Person.Email)
            { PhoneNumber = "09944853827", };


        var accessToken = new JwtAccessTokenModel("AccessToken", 1);

        var userManager = Substitute.For<IUserManager>();
        userManager.FindByEmailAsync(userPasswordLoginQuery.UserNameOrEmail, CancellationToken.None)
            .Returns(Task.FromResult<UserEntity?>(userEntity));

        var jwtService = Substitute.For<IJwtService>();
        jwtService.GenerateJwtTokenAsync(Arg.Any<UserEntity>(), CancellationToken.None)
            .Returns(Task.FromResult(accessToken));

        //Act
        var userPasswordLoginQueryHandler = new UserPasswordLoginQueryHandler(userManager, jwtService);
        var validationBehavior =
            new ValidateRequestBehavior<UserPasswordLoginQuery, OperationResult<JwtAccessTokenModel>>(
                _serviceProvider.GetRequiredService<IValidator<UserPasswordLoginQuery>>());
        var userPasswordLoginResult = await validationBehavior.Handle(userPasswordLoginQuery, CancellationToken.None,
            userPasswordLoginQueryHandler.Handle);

        //Assertion
        userPasswordLoginResult.Result.Should().Be(null);
        userPasswordLoginResult.IsSuccess.Should().Be(false);

        _testOutputHelper.WriteLineOperationResultErrors(userPasswordLoginResult);
    }
}