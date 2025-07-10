using FluentAssertions;
using Shopping.Domain.Entities.Product;
using Shopping.Domain.Entities.User;

namespace Shoping.Domain.Test.UserTests;

public class UserEntityTest
{
    [Fact]
    public void Add_Product_To_User_Should_Have_1_Count()
    {
        var user = new UserEntity("ali", "hm", "arhm", "ali@hm.com");
        var product = ProductEntity.Create("Title", "Description",200,20,ProductEntity.ProductState.Active,user.Id,Guid.NewGuid());

        user.AddProduct(product);

        user.Products.Should().HaveCount(1);
    }
}