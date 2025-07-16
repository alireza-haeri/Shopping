using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Application.Contracts.FileService.Models;
using Shopping.Application.Contracts.User;
using Shopping.Application.Extensions;
using Shopping.Application.Features.Product.Commands;
using Shopping.Application.Repositories.Category;
using Shopping.Application.Repositories.Common;
using Shopping.Application.Repositories.Product;
using Shopping.Application.Test.Extensions;
using Shopping.Domain.Entities.Product;
using Shopping.Domain.Entities.User;
using Xunit.Abstractions;

namespace Shopping.Application.Test;

public class ProductFeaturesTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IServiceProvider _serviceProvider;

    public ProductFeaturesTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterApplicationValidator();
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task Create_Product_With_Valid_Parameters_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var command = new CreateProductCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            2000,
            2,
            ProductEntity.ProductState.Active,
            [new CreateProductCommand.CreateProductImagesModel("Test", "image/png")]);

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var userManagerMock = Substitute.For<IUserManager>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.CreateAsync(Arg.Any<ProductEntity>(), CancellationToken.None)
            .Returns(Task.FromResult);
        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>())!
            .Returns(Task.FromResult(new CategoryEntity("category")));
        userManagerMock.FindByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult(
                new UserEntity(faker.Person.FirstName, faker.Person.LastName, faker.Person.UserName,
                    faker.Person.Email)));
        fileServiceMock.SaveFilesAsync(Arg.Any<List<SaveFileModel>>(), CancellationToken.None)
            .Returns(Task.FromResult(new List<SaveFileResultModel>()));
        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);

        //Act
        var handler = new CreateProductCommandHandler(unitOfWorkMuck, fileServiceMock, userManagerMock);
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeTrue();
        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    [Fact]
    public async Task Create_Product_With_Not_Valid_UserID_Should_Be_Failer()
    {
        //Arrange
        var faker = new Faker();
        var command = new CreateProductCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            2000,
            2,
            ProductEntity.ProductState.Active,
            [new CreateProductCommand.CreateProductImagesModel("Test", "image/png")]);

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var userManagerMock = Substitute.For<IUserManager>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.CreateAsync(Arg.Any<ProductEntity>(), CancellationToken.None)
            .Returns(Task.FromResult);
        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>())!
            .Returns(Task.FromResult(new CategoryEntity("category")));
        userManagerMock.FindByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult<UserEntity>(null!));
        fileServiceMock.SaveFilesAsync(Arg.Any<List<SaveFileModel>>(), CancellationToken.None)
            .Returns(Task.FromResult(new List<SaveFileResultModel>()));
        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);

        //Act
        var handler = new CreateProductCommandHandler(unitOfWorkMuck, fileServiceMock, userManagerMock);
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeFalse();
        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    [Fact]
    public async Task Create_Product_With_Not_Valid_CategoryId_Should_Be_Failer()
    {
        //Arrange
        var faker = new Faker();
        var command = new CreateProductCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            2000,
            2,
            ProductEntity.ProductState.Active,
            [new CreateProductCommand.CreateProductImagesModel("Test", "image/png")]);

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var userManagerMock = Substitute.For<IUserManager>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.CreateAsync(Arg.Any<ProductEntity>(), CancellationToken.None)
            .Returns(Task.FromResult);
        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>())!
            .Returns(Task.FromResult<CategoryEntity>(null!));
        userManagerMock.FindByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult<UserEntity>(null!));
        fileServiceMock.SaveFilesAsync(Arg.Any<List<SaveFileModel>>(), CancellationToken.None)
            .Returns(Task.FromResult(new List<SaveFileResultModel>()));
        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);

        //Act
        var handler = new CreateProductCommandHandler(unitOfWorkMuck, fileServiceMock, userManagerMock);
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeFalse();
        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    [Fact]
    public async Task Create_Product_With_Null_Images_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var command = new CreateProductCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            2000,
            2,
            ProductEntity.ProductState.Active,
            null);

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var userManagerMock = Substitute.For<IUserManager>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.CreateAsync(Arg.Any<ProductEntity>(), CancellationToken.None)
            .Returns(Task.FromResult);
        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>())!
            .Returns(Task.FromResult(new CategoryEntity("category")));
        userManagerMock.FindByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult(
                new UserEntity(faker.Person.FirstName, faker.Person.LastName, faker.Person.UserName,
                    faker.Person.Email)));
        fileServiceMock.SaveFilesAsync(Arg.Any<List<SaveFileModel>>(), CancellationToken.None)
            .Returns(Task.FromResult(new List<SaveFileResultModel>()));
        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);

        //Act
        var handler = new CreateProductCommandHandler(unitOfWorkMuck, fileServiceMock, userManagerMock);
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeTrue();
        _testOutputHelper.WriteLineOperationResultErrors(result);
    }
}