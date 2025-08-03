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

/// <summary>
/// Base class for Product feature tests to handle common setup and mocking.
/// </summary>
public abstract class ProductFeaturesTestBase
{
    protected readonly ITestOutputHelper TestOutputHelper;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IUnitOfWork UnitOfWorkMock;
    protected readonly IProductRepository ProductRepositoryMock;
    protected readonly ICategoryRepository CategoryRepositoryMock;
    protected readonly IUserManager UserManagerMock;
    protected readonly IFileService FileServiceMock;
    protected readonly IMapper Mapper;
    protected readonly Faker Faker;

    protected ProductFeaturesTestBase(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
        Faker = new Faker();

        // Configure services and build the service provider for validators and AutoMapper
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .RegisterApplicationValidator()
            .AddApplicationAutoMapper();
        ServiceProvider = serviceCollection.BuildServiceProvider();
        Mapper = ServiceProvider.GetRequiredService<IMapper>();

        // Setup mock objects for dependencies
        UnitOfWorkMock = Substitute.For<IUnitOfWork>();
        ProductRepositoryMock = Substitute.For<IProductRepository>();
        CategoryRepositoryMock = Substitute.For<ICategoryRepository>();
        UserManagerMock = Substitute.For<IUserManager>();
        FileServiceMock = Substitute.For<IFileService>();

        // Link repositories to the Unit of Work mock
        UnitOfWorkMock.ProductRepository.Returns(ProductRepositoryMock);
        UnitOfWorkMock.CategoryRepository.Returns(CategoryRepositoryMock);
    }
}

public class ProductFeaturesTests
{
    public class CreateProductTests : ProductFeaturesTestBase
    {
        public CreateProductTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public async Task Handle_WithValidParameters_ShouldReturnSuccess()
        {
            // Arrange
            var command = new CreateProductCommand(
                Guid.NewGuid(), Guid.NewGuid(), Faker.Commerce.ProductName(), Faker.Lorem.Sentence(),
                100, 5, ProductEntity.ProductState.Active,
                [new CreateProductCommand.CreateProductImagesModel("base64", "image/png")]);

            CategoryRepositoryMock.GetByIdAsync(command.CategoryId!.Value)!
                .Returns(new CategoryEntity("Electronics"));
            UserManagerMock.FindByIdAsync(command.UserId, CancellationToken.None)!
                .Returns(new UserEntity(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.UserName(), Faker.Internet.Email()));
            FileServiceMock.SaveFilesAsync(Arg.Any<List<SaveFileModel>>(), CancellationToken.None)
                .Returns(new List<SaveFileResultModel> { new("filename.png", "image/png") });

            var handler = new CreateProductCommandHandler(UnitOfWorkMock, FileServiceMock, UserManagerMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, ServiceProvider);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().BeTrue();
            await UnitOfWorkMock.Received(1).CommitAsync(CancellationToken.None);
            TestOutputHelper.WriteLineOperationResultErrors(result);
        }

        [Fact]
        public async Task Handle_WhenCategoryIsNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new CreateProductCommand(
                Guid.NewGuid(), Guid.NewGuid(), "Test", "Test", 100, 1, ProductEntity.ProductState.Active, null);

            CategoryRepositoryMock.GetByIdAsync(command.CategoryId!.Value)!
                .Returns(Task.FromResult<CategoryEntity?>(null));
            UserManagerMock.FindByIdAsync(command.UserId, CancellationToken.None)!
                .Returns(new UserEntity("f", "l", "u", "e"));

            var handler = new CreateProductCommandHandler(UnitOfWorkMock, FileServiceMock, UserManagerMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, ServiceProvider);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessages.Should().Contain(e => e.Value == "Category not found");
            TestOutputHelper.WriteLineOperationResultErrors(result);
        }
        
        [Fact]
        public async Task Handle_WhenUserIsNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new CreateProductCommand(
                Guid.NewGuid(), Guid.NewGuid(), "Test", "Test", 100, 1, ProductEntity.ProductState.Active, null);

            CategoryRepositoryMock.GetByIdAsync(command.CategoryId!.Value)!
                .Returns(new CategoryEntity("Category"));
            UserManagerMock.FindByIdAsync(command.UserId, CancellationToken.None)!
                .Returns(Task.FromResult<UserEntity?>(null));

            var handler = new CreateProductCommandHandler(UnitOfWorkMock, FileServiceMock, UserManagerMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, ServiceProvider);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessages.Should().Contain(e => e.Value == "User not found");
            TestOutputHelper.WriteLineOperationResultErrors(result);
        }

        [Fact]
        public async Task Handle_WithInvalidDomainData_ShouldReturnDomainFailureResult()
        {
            // Arrange
            var command = new CreateProductCommand(
                Guid.NewGuid(), Guid.NewGuid(), Faker.Commerce.ProductName(), Faker.Lorem.Sentence(),
                -100, 5, ProductEntity.ProductState.Active, null); // Invalid Price

             CategoryRepositoryMock.GetByIdAsync(command.CategoryId!.Value)!
                .Returns(new CategoryEntity("Electronics"));
            UserManagerMock.FindByIdAsync(command.UserId, CancellationToken.None)!
                .Returns(new UserEntity(Faker.Name.FirstName(), Faker.Name.LastName(), Faker.Internet.UserName(), Faker.Internet.Email()));

            var handler = new CreateProductCommandHandler(UnitOfWorkMock, FileServiceMock, UserManagerMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, ServiceProvider);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessages.Should().Contain(e => e.Key.Contains("Price"));
            TestOutputHelper.WriteLineOperationResultErrors(result);
        }
    }
    
    public class EditProductTests : ProductFeaturesTestBase
    {
        public EditProductTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public async Task Handle_WithValidParameters_ShouldReturnSuccess()
        {
            // Arrange
            var command = new EditProductCommand(
                Guid.NewGuid(), "Updated Title", "Updated Desc", 150, 10,
                ProductEntity.ProductState.Hidden, Guid.NewGuid(), null, null);

            var existingProduct = ProductEntity.Create(
                "Old Title", "Old Desc", 100, 5, ProductEntity.ProductState.Active, Guid.NewGuid(), Guid.NewGuid());
            
            ProductRepositoryMock.GetByIdAsTrackAsync(command.ProductId, CancellationToken.None)!
                .Returns(existingProduct);
            CategoryRepositoryMock.GetByIdAsync(command.CategoryId!.Value)!
                .Returns(new CategoryEntity("New Category"));
            
            var handler = new EditProductCommandHandler(UnitOfWorkMock, FileServiceMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, ServiceProvider);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existingProduct.Title.Should().Be(command.Title);
            existingProduct.Description.Should().Be(command.Description);
            existingProduct.Price.Should().Be(command.Price);
            existingProduct.Quantity.Should().Be(command.Quantity);
            existingProduct.State.Should().Be(command.State!.Value);
            existingProduct.CategoryId.Should().Be(command.CategoryId!.Value);
            await UnitOfWorkMock.Received(1).CommitAsync(CancellationToken.None);
            TestOutputHelper.WriteLineOperationResultErrors(result);
        }
        
        [Fact]
        public async Task Handle_WhenProductIsNotFound_ShouldReturnFailureResult()
        {
            // Arrange
             var command = new EditProductCommand(
                Guid.NewGuid(), "Updated Title", "Updated Desc", 150, 10,
                ProductEntity.ProductState.Hidden, Guid.NewGuid(), null, null);

            ProductRepositoryMock.GetByIdAsTrackAsync(command.ProductId, CancellationToken.None)!
                .Returns(Task.FromResult<ProductEntity?>(null));
            CategoryRepositoryMock.GetByIdAsync(Arg.Any<Guid>())
                .Returns(new CategoryEntity("Electronics"));
            
            var handler = new EditProductCommandHandler(UnitOfWorkMock, FileServiceMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, ServiceProvider);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessages.Should().Contain(e => e.Value == "Product not found");
            TestOutputHelper.WriteLineOperationResultErrors(result);
        }
    }
    
    public class DeleteProductTests : ProductFeaturesTestBase
    {
        public DeleteProductTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public async Task Handle_WithValidProductId_ShouldReturnSuccess()
        {
            // Arrange
            var command = new DeleteProductCommand(Guid.NewGuid());
            var product = ProductEntity.Create("Title", "Desc", 10, 1, ProductEntity.ProductState.Active, Guid.NewGuid(), null);
            product.AddImage(new ImageValueObject("file.jpg", "image/jpeg", "url"));

            ProductRepositoryMock.GetByIdAsTrackAsync(command.Id, CancellationToken.None)!
                .Returns(product);
            
            var handler = new DeleteProductCommandHandler(UnitOfWorkMock, FileServiceMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, ServiceProvider);
            
            // Assert
            result.IsSuccess.Should().BeTrue();
            await ProductRepositoryMock.Received(1).DeleteAsync(product, CancellationToken.None);
            await UnitOfWorkMock.Received(1).CommitAsync(CancellationToken.None);
            await FileServiceMock.Received(1).RemoveFileAsync(Arg.Is<string[]>(x => x.Contains("file.jpg")), CancellationToken.None);
        }
        
        [Fact]
        public async Task Handle_WhenProductIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var command = new DeleteProductCommand(Guid.NewGuid());
            ProductRepositoryMock.GetByIdAsTrackAsync(command.Id, CancellationToken.None)!
                .Returns(Task.FromResult<ProductEntity?>(null));
            
            var handler = new DeleteProductCommandHandler(UnitOfWorkMock, FileServiceMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(command, handler, ServiceProvider);
            
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsNotFound.Should().BeTrue();
            result.ErrorMessages.Should().Contain(e => e.Value == "Product not found");
        }
    }

    public class GetProductDetailTests : ProductFeaturesTestBase
    {
        public GetProductDetailTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public async Task Handle_WithValidId_ShouldReturnProductDetails()
        {
            // Arrange
            var query = new GetProductDetailByIdQuery(Guid.NewGuid());
            var user = new UserEntity("John", "Doe", "johndoe", "john@test.com");
            var category = new CategoryEntity("Electronics");
            var product = ProductEntity.Create(Guid.NewGuid(), "Laptop", "A good laptop", 1200, 10, ProductEntity.ProductState.Active, user, category);
            product.AddImage(new ImageValueObject("laptop.jpg", "image/jpeg", string.Empty));
            
            ProductRepositoryMock.GetDetailsByIdAsync(query.Id, CancellationToken.None)!
                .Returns(product);
            FileServiceMock.GetFilesByNameAsync(Arg.Any<List<string>>(), CancellationToken.None)
                .Returns(new List<GetFileModel> { new("http://example.com/laptop.jpg", "image/jpeg", "laptop.jpg") });

            var handler = new GetProductDetailByIdQueryHandler(UnitOfWorkMock, FileServiceMock, Mapper);
            
            // Act
            var result = await Helpers.ValidateAndExecuteAsync(query, handler, ServiceProvider);
            
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().NotBeNull();
            result.Result!.ProductId.Should().Be(product.Id);
            result.Result.Title.Should().Be("Laptop");
            result.Result.OwnerFirstName.Should().Be("John");
            result.Result.CategoryTitle.Should().Be("Electronics");
            result.Result.ProductImages.Should().HaveCount(1);
            result.Result.ProductImages.First().ImageUrl.Should().Be("http://example.com/laptop.jpg");
        }
        
        [Fact]
        public async Task Handle_WhenProductIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var query = new GetProductDetailByIdQuery(Guid.NewGuid());
            
            ProductRepositoryMock.GetDetailsByIdAsync(query.Id, CancellationToken.None)!
                .Returns(Task.FromResult<ProductEntity?>(null));

            var handler = new GetProductDetailByIdQueryHandler(UnitOfWorkMock, FileServiceMock, Mapper);
            
            // Act
            var result = await Helpers.ValidateAndExecuteAsync(query, handler, ServiceProvider);
            
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.IsNotFound.Should().BeTrue();
        }
    }

    public class GetProductsTests : ProductFeaturesTestBase
    {
        public GetProductsTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public async Task Handle_WithoutCategoryFilter_ShouldReturnProducts()
        {
            // Arrange
            var query = new GetProductsQuery("test", 1, 10, null);
            var products = new List<ProductEntity>
            {
                ProductEntity.Create("P1", "D1", 10, 1, ProductEntity.ProductState.Active, Guid.NewGuid(), null),
                ProductEntity.Create("P2", "D2", 20, 2, ProductEntity.ProductState.Active, Guid.NewGuid(), null)
            };
            products[0].AddImage(new ImageValueObject("p1.jpg", "image/jpeg", ""));
            
            ProductRepositoryMock.GetProductsAsync(query.Title, query.CurrentPage, query.PageCount, query.CategoryId, CancellationToken.None)!
                .Returns(products);
            FileServiceMock.GetFilesByNameAsync(Arg.Is<List<string>>(x => x.Contains("p1.jpg")), CancellationToken.None)
                .Returns(new List<GetFileModel>{new("p1.jpg", "image/jpeg", "url1")});

            var handler = new GetProductsQueryHandler(UnitOfWorkMock, FileServiceMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(query, handler, ServiceProvider);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result.First(p => p.Title == "P1").ProductImage.Should().NotBeNull();
            result.Result.First(p => p.Title == "P2").ProductImage.Should().BeNull();
        }
        
        [Fact]
        public async Task Handle_WhenCategoryIsNotFound_ShouldReturnFailureResult()
        {
            // Arrange
            var query = new GetProductsQuery("test", 1, 10, Guid.NewGuid());
            
            CategoryRepositoryMock.GetByIdAsync(query.CategoryId!.Value, CancellationToken.None)!
                .Returns(Task.FromResult<CategoryEntity?>(null));

            var handler = new GetProductsQueryHandler(UnitOfWorkMock, FileServiceMock);

            // Act
            var result = await Helpers.ValidateAndExecuteAsync(query, handler, ServiceProvider);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessages.Should().Contain(e => e.Value == "Category not found");
        }
    }
}