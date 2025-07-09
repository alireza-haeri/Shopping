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
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.NewGuid();

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, userId, categoryId);

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
        Guid? userId = null;
        Guid? categoryId = Guid.NewGuid();

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, userId, categoryId);

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
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = null;

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, userId, categoryId);

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
        Guid? userId = Guid.Empty;
        Guid? categoryId = Guid.NewGuid();

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, userId, categoryId);

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
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.Empty;

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, userId, categoryId);

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
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.NewGuid();

        //Act
        Action act = () => ProductEntity.Create(title, description, price, quantity, userId, categoryId);

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
        Guid? id= Guid.NewGuid();
        Guid? userId = Guid.NewGuid();
        Guid? categoryId = Guid.NewGuid();

        //Act
        var product1 =  ProductEntity.Create(id,title, description, price, quantity, userId, categoryId);
        var product2 =  ProductEntity.Create(id,title, description, price, quantity, userId, categoryId);

        //Assert
        product1.Equals(product2).Should().BeTrue();
    }
}