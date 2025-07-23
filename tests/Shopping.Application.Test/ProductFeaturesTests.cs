using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Application.Contracts.FileService.Models;
using Shopping.Application.Contracts.User;
using Shopping.Application.Extensions;
using Shopping.Application.Features.Product.Commands;
using Shopping.Application.Features.Product.Queries;
using Shopping.Application.Repositories.Category;
using Shopping.Application.Repositories.Common;
using Shopping.Application.Repositories.Product;
using Shopping.Application.Test.Extensions;
using Shopping.Domain.Common.ValueObjects;
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
        serviceCollection
            .RegisterApplicationValidator()
            .AddApplicationAutoMapper();

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

    [Fact]
    public async Task Update_Product_With_Valid_Parameters_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var command = new EditProductCommand(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            2,
            ProductEntity.ProductState.Hidden,
            Guid.NewGuid(),
            ["i2"],
            [new EditProductCommand.AddedImagesContent("i3", "image/png")]);

        var product = ProductEntity.Create(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            3,
            ProductEntity.ProductState.Active,
            Guid.NewGuid(),
            Guid.NewGuid());
        var category = new CategoryEntity(faker.Lorem.Sentence(2));

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.GetByIdAsTrackAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult(product));

        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult(category));

        fileServiceMock.SaveFilesAsync(Arg.Any<List<SaveFileModel>>(), CancellationToken.None)
            .Returns(Task.FromResult(new List<SaveFileResultModel>()
                { new("i1", "image/png"), new SaveFileResultModel("i2", "image/png") }));

        fileServiceMock.RemoveFileAsync(Arg.Any<string[]>(), CancellationToken.None)
            .Returns(Task.FromResult);

        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);

        var handler = new EditProductCommandHandler(unitOfWorkMuck, fileServiceMock);

        //Act
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeTrue();
        product.Title.Should().Be(command.Title);
        product.Description.Should().Be(command.Description);
        product.CategoryId.Should().Be(command.CategoryId!.Value);
        product.State.Should().Be(command.State);
        product.Price.Should().Be(command.Price);
        product.Quantity.Should().Be(2);
        product.Images.Should().HaveCount(2);

        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    [Fact]
    public async Task Update_Product_With_Not_Valid_Category_Should_Be_Failer()
    {
        //Arrange
        var faker = new Faker();
        var command = new EditProductCommand(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            2,
            ProductEntity.ProductState.Hidden,
            Guid.NewGuid(),
            ["i1"],
            [new EditProductCommand.AddedImagesContent("i3", "image/png")]);

        var product = ProductEntity.Create(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            3,
            ProductEntity.ProductState.Active,
            Guid.NewGuid(),
            Guid.NewGuid());
        var category = new CategoryEntity(faker.Lorem.Sentence(2));

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.GetByIdAsTrackAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult(product));

        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult<CategoryEntity?>(null));

        fileServiceMock.SaveFilesAsync(Arg.Any<List<SaveFileModel>>(), CancellationToken.None)
            .Returns(Task.FromResult(new List<SaveFileResultModel>() { new("i1", "image/png") }));

        fileServiceMock.RemoveFileAsync(Arg.Any<string[]>(), CancellationToken.None)
            .Returns(Task.FromResult);

        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);

        var handler = new EditProductCommandHandler(unitOfWorkMuck, fileServiceMock);

        //Act
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeFalse();

        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    [Fact]
    public async Task Update_Product_With_Null_Removed_Image_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var command = new EditProductCommand(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            2,
            ProductEntity.ProductState.Hidden,
            Guid.NewGuid(),
            null,
            [new EditProductCommand.AddedImagesContent("i3", "image/png")]);

        var product = ProductEntity.Create(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            3,
            ProductEntity.ProductState.Active,
            Guid.NewGuid(),
            Guid.NewGuid());

        var images = new List<SaveFileResultModel>() { new("i1", "image/png") };

        images.ForEach(i => product.AddImage(new ImageValueObject(i.FileName, i.FileType, string.Empty)));

        var category = new CategoryEntity(faker.Lorem.Sentence(2));

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.GetByIdAsTrackAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult(product));

        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult<CategoryEntity?>(category));

        fileServiceMock.SaveFilesAsync(Arg.Any<List<SaveFileModel>>(), CancellationToken.None)
            .Returns(Task.FromResult(product.Images.Select(i => new SaveFileResultModel(i.FileName, i.ImageType))
                .ToList()));

        fileServiceMock.RemoveFileAsync(Arg.Any<string[]>(), CancellationToken.None)
            .Returns(Task.FromResult);

        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);

        var handler = new EditProductCommandHandler(unitOfWorkMuck, fileServiceMock);

        //Act
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeTrue();
        product.Title.Should().Be(command.Title);
        product.Description.Should().Be(command.Description);
        product.CategoryId.Should().Be(command.CategoryId!.Value);
        product.State.Should().Be(command.State);
        product.Price.Should().Be(command.Price);
        product.Quantity.Should().Be(2);
        product.Images.Should().HaveCount(2);

        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    [Fact]
    public async Task Update_Product_With_Null_Added_Image_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var command = new EditProductCommand(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            2,
            ProductEntity.ProductState.Hidden,
            Guid.NewGuid(),
            null,
            null);

        var product = ProductEntity.Create(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            3,
            ProductEntity.ProductState.Active,
            Guid.NewGuid(),
            Guid.NewGuid());

        var images = new List<SaveFileResultModel>() { new("i1", "image/png") };

        images.ForEach(i => product.AddImage(new ImageValueObject(i.FileName, i.FileType, string.Empty)));

        var category = new CategoryEntity(faker.Lorem.Sentence(2));

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.GetByIdAsTrackAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult(product));

        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult<CategoryEntity?>(category));

        fileServiceMock.SaveFilesAsync(Arg.Any<List<SaveFileModel>>(), CancellationToken.None)
            .Returns(Task.FromResult(product.Images.Select(i => new SaveFileResultModel(i.FileName, i.ImageType))
                .ToList()));

        fileServiceMock.RemoveFileAsync(Arg.Any<string[]>(), CancellationToken.None)
            .Returns(Task.FromResult);

        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);

        var handler = new EditProductCommandHandler(unitOfWorkMuck, fileServiceMock);

        //Act
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeTrue();
        product.Title.Should().Be(command.Title);
        product.Description.Should().Be(command.Description);
        product.CategoryId.Should().Be(command.CategoryId!.Value);
        product.State.Should().Be(command.State);
        product.Price.Should().Be(command.Price);
        product.Quantity.Should().Be(2);
        product.Images.Should().HaveCount(1);

        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    [Fact]
    public async Task Getting_Product_Details_With_Valid_Parameters_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var query = new GetProductDetailByIdQuery(Guid.NewGuid());


        var userMock = new UserEntity(faker.Person.FirstName, faker.Person.LastName, faker.Person.UserName,
            faker.Person.Email)
        {
            PhoneNumber = "09921768367"
        };

        var category = new CategoryEntity(faker.Lorem.Sentence(2));

        var product = ProductEntity.Create(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            3,
            ProductEntity.ProductState.Active,
            userMock,
            category);

        var images = new List<SaveFileResultModel>() { new("i1", "image/png") };

        images.ForEach(i => product.AddImage(new ImageValueObject(i.FileName, i.FileType, string.Empty)));

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.GetDetailsByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult(product));

        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult<CategoryEntity?>(category));

        fileServiceMock.GetFilesByNameAsync(Arg.Any<List<string>>(), CancellationToken.None)
            .Returns(Task.FromResult(images.Select(i => new GetFileModel(i.FileName, i.FileType, i.FileName))
                .ToList()));

        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);
        var mapper = _serviceProvider.GetRequiredService<IMapper>();
        var handler = new GetProductDetailByIdQueryHandler(unitOfWorkMuck, fileServiceMock, mapper);

        //Act
        var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

        //Assertion
        result.IsSuccess.Should().Be(true);

        result.Result!.ProductId.Should().Be(product.Id);
        result.Result!.Title.Should().Be(product.Title);
        result.Result.Description.Should().Be(product.Description);
        result.Result.Price.Should().Be(product.Price);
        result.Result.Quantity.Should().Be(product.Quantity);
        result.Result.State.Should().Be(product.State);
        result.Result.CategoryTitle.Should().Be(product.Category.Title);
        result.Result.CategoryId.Should().Be(product.CategoryId);
        result.Result.OwnerId.Should().Be(product.UserId);
        result.Result.OwnerFirstName.Should().Be(product.User.FirstName);
        result.Result.OwnerLastName.Should().Be(product.User.LastName);
        result.Result.OwnerPhone.Should().Be(product.User.PhoneNumber);
        result.Result.OwnerEmail.Should().Be(product.User.Email);
        result.Result.OwnerUserName.Should().Be(product.User.UserName);
        result.Result.ProductImages.Should().HaveCount(images.Count);

        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    [Fact]
    public async Task Getting_Products_With_Valid_Parameters_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var query = new GetProductsQuery(faker.Lorem.Sentence(3), 1, 3, Guid.NewGuid());

        var category = new CategoryEntity(faker.Lorem.Sentence(2));
        var images = new List<SaveFileResultModel>() { new("i1", "image/png") };

        List<ProductEntity> products =
        [
            ProductEntity.Create(
                Guid.NewGuid(),
                faker.Lorem.Sentence(2),
                faker.Lorem.Sentence(5),
                decimal.Parse(faker.Commerce.Price()),
                3,
                ProductEntity.ProductState.Active,
                Guid.NewGuid(),
                Guid.NewGuid()),
            ProductEntity.Create(
                Guid.NewGuid(),
                faker.Lorem.Sentence(2),
                faker.Lorem.Sentence(5),
                decimal.Parse(faker.Commerce.Price()),
                3,
                ProductEntity.ProductState.Active,
                Guid.NewGuid(),
                Guid.NewGuid()),
        ];


        foreach (var product in products)
        {
            images.ForEach(i => product.AddImage(new ImageValueObject(i.FileName, i.FileType, string.Empty)));
        }

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.GetProductsAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid>(),
                CancellationToken.None)!
            .Returns(Task.FromResult(products));

        categoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult<CategoryEntity?>(category));

        fileServiceMock.GetFilesByNameAsync(Arg.Any<List<string>>(), CancellationToken.None)
            .Returns(Task.FromResult(images.Select(i => new GetFileModel(i.FileName, i.FileType, i.FileName))
                .ToList()));

        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);
        var handler = new GetProductsQueryHandler(unitOfWorkMuck, fileServiceMock);

        //Act
        var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

        //Assertion
        result.IsSuccess.Should().Be(true);

        result.Result.Should().AllSatisfy(p =>
        {
            p.Title.Should().NotBeNullOrEmpty();
            p.Price.Should().BeGreaterThan(0);
            p.Quantity.Should().BeGreaterThan(0);
            p.ProductImage.Should().NotBeNull();
        });

        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    [Fact]
    public async Task Getting_Products_With_Null_CategoryId_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var query = new GetProductsQuery(faker.Lorem.Sentence(3), 1, 3);

        var images = new List<SaveFileResultModel>() { new("i1", "image/png") };

        List<ProductEntity> products =
        [
            ProductEntity.Create(
                Guid.NewGuid(),
                faker.Lorem.Sentence(2),
                faker.Lorem.Sentence(5),
                decimal.Parse(faker.Commerce.Price()),
                3,
                ProductEntity.ProductState.Active,
                Guid.NewGuid(),
                Guid.NewGuid()),
            ProductEntity.Create(
                Guid.NewGuid(),
                faker.Lorem.Sentence(2),
                faker.Lorem.Sentence(5),
                decimal.Parse(faker.Commerce.Price()),
                3,
                ProductEntity.ProductState.Active,
                Guid.NewGuid(),
                Guid.NewGuid()),
        ];


        foreach (var product in products)
        {
            images.ForEach(i => product.AddImage(new ImageValueObject(i.FileName, i.FileType, string.Empty)));
        }

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.GetProductsAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<Guid?>(),
                CancellationToken.None)!
            .Returns(Task.FromResult(products));

        fileServiceMock.GetFilesByNameAsync(Arg.Any<List<string>>(), CancellationToken.None)
            .Returns(Task.FromResult(images.Select(i => new GetFileModel(i.FileName, i.FileType, i.FileName))
                .ToList()));

        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);
        unitOfWorkMuck.CategoryRepository.Returns(categoryRepositoryMock);
        var handler = new GetProductsQueryHandler(unitOfWorkMuck, fileServiceMock);

        //Act
        var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

        //Assertion
        result.IsSuccess.Should().Be(true);

        result.Result.Should().AllSatisfy(p =>
        {
            p.Title.Should().NotBeNullOrEmpty();
            p.Price.Should().BeGreaterThan(0);
            p.Quantity.Should().BeGreaterThan(0);
            p.ProductImage.Should().NotBeNull();
        });

        _testOutputHelper.WriteLineOperationResultErrors(result);
    }
    
    [Fact]
    public async Task Delete_Product_With_Valid_Parameters_Should_Be_Success()
    {
        //Arrange
        var faker = new Faker();
        var command = new DeleteProductCommand(Guid.NewGuid());

        var product = ProductEntity.Create(
            Guid.NewGuid(),
            faker.Lorem.Sentence(2),
            faker.Lorem.Sentence(5),
            decimal.Parse(faker.Commerce.Price()),
            3,
            ProductEntity.ProductState.Active,
            Guid.NewGuid(),
            Guid.NewGuid());

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.GetByIdAsTrackAsync(Arg.Any<Guid>(), CancellationToken.None)!
            .Returns(Task.FromResult(product));
        productRepositoryMock.DeleteAsync(Arg.Any<ProductEntity>(), CancellationToken.None)
            .Returns(Task.CompletedTask);
        fileServiceMock.RemoveFileAsync(Arg.Any<string[]>(), CancellationToken.None)
            .Returns(Task.FromResult);
        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);

        //Act
        var handler = new DeleteProductCommandHandler(unitOfWorkMuck, fileServiceMock);
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeTrue();
        _testOutputHelper.WriteLineOperationResultErrors(result);
    }
    
    [Fact]
    public async Task Delete_Product_With_Null_ProductId_Should_Be_Success()
    {
        //Arrange
        var command = new DeleteProductCommand(Guid.Empty);

        var unitOfWorkMuck = Substitute.For<IUnitOfWork>();
        var productRepositoryMock = Substitute.For<IProductRepository>();
        var fileServiceMock = Substitute.For<IFileService>();

        productRepositoryMock.GetByIdAsTrackAsync(Arg.Any<Guid>(), CancellationToken.None)
            .Returns(Task.FromResult<ProductEntity?>(null));
        productRepositoryMock.DeleteAsync(Arg.Any<ProductEntity>(), CancellationToken.None)
            .Returns(Task.CompletedTask);
        fileServiceMock.RemoveFileAsync(Arg.Any<string[]>(), CancellationToken.None)
            .Returns(Task.FromResult);
        unitOfWorkMuck.ProductRepository.Returns(productRepositoryMock);

        //Act
        var handler = new DeleteProductCommandHandler(unitOfWorkMuck, fileServiceMock);
        var result = await Helpers.ValidateAndExecuteAsync(command, handler, _serviceProvider);

        //Assertion
        result.Result.Should().BeFalse();
        result.ErrorMessages.Should().NotBeNullOrEmpty();
        
        _testOutputHelper.WriteLineOperationResultErrors(result);
    }
}