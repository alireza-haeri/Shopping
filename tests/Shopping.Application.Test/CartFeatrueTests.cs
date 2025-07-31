using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MockQueryable;
using MockQueryable.NSubstitute; // <-- قدم ۱: این using را اضافه کنید
using NSubstitute;
using Shopping.Application.Contracts;
using Shopping.Application.Contracts.User;
using Shopping.Application.Extensions;
using Shopping.Application.Features.Cart.Queries;
using Shopping.Application.Repositories.Common;
using Shopping.Application.Test.Extensions;
using Shopping.Domain.Entities.Cart;
using Shopping.Domain.Entities.Product;
using Shopping.Domain.Entities.User;
using Xunit.Abstractions;

namespace Shopping.Application.Test;

public class CartFeatureTests
{
    // ... (تمام پراپرتی‌ها و سازنده بدون تغییر باقی می‌مانند)
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IServiceProvider _serviceProvider;
    private readonly Faker _faker;

    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IUserManager _userManagerMock;
    private readonly IReadDbContext _readDbContextMock;

    public CartFeatureTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _faker = new Faker();

        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterApplicationValidator();
        _serviceProvider = serviceCollection.BuildServiceProvider();

        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _userManagerMock = Substitute.For<IUserManager>();
        _readDbContextMock = Substitute.For<IReadDbContext>();
    }
    
    // ... (تست‌های Command و متدهای Helper بدون تغییر باقی می‌مانند)
    #region Helper Methods

    private ProductEntity CreateFakeProduct(int quantity = 10)
    {
        return ProductEntity.Create(
            _faker.Commerce.ProductName(),
            _faker.Lorem.Sentence(),
            decimal.Parse(_faker.Commerce.Price(10, 1000, 2)),
            quantity,
            ProductEntity.ProductState.Active,
            Guid.NewGuid(),
            Guid.NewGuid());
    }
    private UserEntity CreateFakeUser() => new(
        _faker.Person.FirstName,
        _faker.Person.LastName,
        _faker.Internet.UserName(),
        _faker.Internet.Email());
    #endregion
    
    #region GetCartDetailsQuery Tests

    [Fact]
    public async Task GetCartDetails_WhenCartExists_ShouldReturnCorrectDto()
    {
        //Arrange
        var user = CreateFakeUser();
        var product = CreateFakeProduct();
        var cart = CartEntity.Create(user.Id);
        cart.AddItem(product.Id, 2, 125.50m);
        var cartItem = cart.Items.First();
        typeof(CartItemEntity).GetProperty(nameof(CartItemEntity.Product))!.SetValue(cartItem, product);

        var cartsList = new List<CartEntity> { cart };
        var query = new GetCartDetailsQuery(user.Id);

        // --- راه حل کلیدی ---
        var mockCarts = cartsList.BuildMock().AsQueryable(); // <-- قدم ۲: استفاده از BuildMock
        _readDbContextMock.Carts.Returns(mockCarts);
        
        var handler = new GetCartDetailsQueryHandler(_readDbContextMock);

        //Act
        var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

        //Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Result;
        dto.Should().NotBeNull();
        dto.CartId.Should().Be(cart.Id);
        
        _testOutputHelper.WriteLineOperationResultErrors(result);
    }
    
    [Fact]
    public async Task GetCartDetails_WhenCartDoesNotExist_ShouldReturnNullResult()
    {
        //Arrange
        var user = CreateFakeUser();
        var query = new GetCartDetailsQuery(user.Id);
        
        var emptyCartsList = new List<CartEntity>();
        
        // --- راه حل کلیدی ---
       var mockCarts = emptyCartsList.BuildMock().AsQueryable(); // <-- قدم ۲: استفاده از BuildMock()
        _readDbContextMock.Carts.Returns(mockCarts);

        var handler = new GetCartDetailsQueryHandler(_readDbContextMock);
        
        //Act
        var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().BeNull();
        
        _testOutputHelper.WriteLineOperationResultErrors(result);
    }

    #endregion

    #region GetCartItemCountQuery Tests
    
    [Fact]
    public async Task GetCartItemCount_WhenCartHasItems_ShouldReturnSumOfQuantities()
    {
        //Arrange
        var user = CreateFakeUser();
        var cart = CartEntity.Create(user.Id);
        cart.AddItem(Guid.NewGuid(), 3, 10);
        cart.AddItem(Guid.NewGuid(), 5, 20);
        
        var cartsList = new List<CartEntity> { cart };
        var query = new GetCartItemCountQuery(user.Id);
        
        // --- راه حل کلیدی ---
        var mockCarts = cartsList.BuildMock().AsQueryable(); // <-- قدم ۲: استفاده از BuildMock()
        _readDbContextMock.Carts.Returns(mockCarts);
        
        var handler = new GetCartItemCountQueryHandler(_readDbContextMock);

        //Act
        var result = await Helpers.ValidateAndExecuteAsync(query, handler, _serviceProvider);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be(8);
        
        _testOutputHelper.WriteLineOperationResultErrors(result);
    }
    
    #endregion
}