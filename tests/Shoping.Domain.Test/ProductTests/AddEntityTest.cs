using FluentAssertions;
using Shopping.Domain.Entities.Product;

namespace Shoping.Domain.Test.ProductTests;

public class AddEntityTest
{
    [Fact]
    public void Creating_Ads_With_Null_Title_Should_Throw_ArgumentNullException()
    {
        //Arrange
        string? title = null;
        var description = "Product Description";
        var price = 4000;
        var quantity = 1;
        var state = ProductEntity.ProductState.Active;
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.NewGuid();

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, state, userId, categoryId);

        //Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Creating_Ads_With_Null_User_Should_Throw_ArgumentNullException()
    {
        //Arrange
        var title = "Product Title";
        var description = "Product Description";
        var price = 4000;
        var quantity = 1;
        var state = ProductEntity.ProductState.Active;
        Guid? userId = null;
        Guid? categoryId = Guid.NewGuid();

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, state, userId, categoryId);

        //Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Creating_Ads_With_Null_Category_Should_Throw_ArgumentNullException()
    {
        //Arrange
        var title = "Product Title";
        var description = "Product Description";
        var price = 4000;
        var quantity = 1;
        var state = ProductEntity.ProductState.Active;
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = null;

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, state, userId, categoryId);

        //Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Creating_Ads_With_Empty_User_Should_Throw_ArgumentNullException()
    {
        //Arrange
        var title = "Product Title";
        var description = "Product Description";
        var price = 4000;
        var quantity = 1;
        var state = ProductEntity.ProductState.Active;
        Guid? userId = Guid.Empty;
        Guid? categoryId = Guid.NewGuid();

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, state, userId, categoryId);

        //Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Creating_Ads_With_Empty_Category_Should_Throw_ArgumentNullException()
    {
        //Arrange
        var title = "Product Title";
        var description = "Product Description";
        var price = 4000;
        var quantity = 1;
        var state = ProductEntity.ProductState.Active;
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.Empty;

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, state, userId, categoryId);

        //Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Creating_Ads_With_LessThan_Zero_Quantity_Should_Throw_ArgumentNullException()
    {
        //Arrange
        var title = "Product Title";
        var description = "Product Description";
        var price = 4000;
        var quantity = -1;
        var state = ProductEntity.ProductState.Active;
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.NewGuid();

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, state, userId, categoryId);

        //Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Two_Categories_With_SameId_Must_Be_Equal()
    {
        //Arrange
        var title = "Product Title";
        var description = "Product Description";
        var price = 4000;
        var quantity = 1;
        var state = ProductEntity.ProductState.Active;
        Guid? id = Guid.NewGuid();
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.NewGuid();

        //Act
        var product1 = ProductEntity.Create(id, title, description, price, quantity, state, userId, categoryId);
        var product2 = ProductEntity.Create(id, title, description, price, quantity, state, userId, categoryId);

        //Assert
        product1.Equals(product2).Should().BeTrue();
    }
    
    [Fact]
    public void Creating_An_Product_Should_Have_ChangeLog()
    {
        //Arrange
        string? title = "Product Title";
        var description = "Product Description";
        var price = 4000;
        var quantity = 1;
        var state = ProductEntity.ProductState.Active;
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.NewGuid();

        //Act
        var product = ProductEntity.Create(title, description, price, quantity, state, userId, categoryId);

        //Assert
        product.ChangeLogs.Should().HaveCount(1);
    }
    
    [Fact]
    public void Changing_Product_State_Should_Have_GreaterThan_1_ChangeLog()
    {
        //Arrange
        string? title = "Product Title";
        var description = "Product Description";
        var price = 4000;
        var quantity = 1;
        var state = ProductEntity.ProductState.Active;
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.NewGuid();

        //Act
        var product = ProductEntity.Create(title, description, price, quantity, state, userId, categoryId);
        product.ChangeState(ProductEntity.ProductState.Hidden);

        //Assert
        product.ChangeLogs.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void Edit_Product_Should_Have_GreaterThan_1_ChangeLog()
    {
        //Arrange
        string? title = "Product Title";
        var description = "Product Description";
        var price = 4000;
        var quantity = 1;
        var state = ProductEntity.ProductState.Active;
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.NewGuid();

        //Act
        var product = ProductEntity.Create(title, description, price, quantity, state, userId, categoryId);
        product.Edit("New Title", description, price,quantity, categoryId);

        //Assert
        product.ChangeLogs.Should().HaveCountGreaterThan(1);
    }
}