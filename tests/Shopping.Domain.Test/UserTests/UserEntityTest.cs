using FluentAssertions;
using Shopping.Domain.Entities.Product;
using Shopping.Domain.Entities.User;

namespace Shoping.Domain.Test.UserTests
{
    /// <summary>
    /// Contains comprehensive unit tests for the UserEntity to verify
    /// its construction, property initialization, and methods.
    /// </summary>
    public class UserEntityTests
    {
        #region Test Setup

        /// <summary>
        /// Creates a valid UserEntity instance for use as the System Under Test (SUT).
        /// This centralizes user creation to keep tests DRY (Don't Repeat Yourself).
        /// </summary>
        private UserEntity CreateUserSut()
        {
            return new UserEntity(
                firstName: "Saeed",
                lastName: "Molaii",
                userName: "saeedmolaii",
                email: "saeed.molaii@example.com");
        }

        #endregion

        #region Constructor and Initialization Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var userName = "johndoe";
            var email = "john.doe@test.com";

            // Act
            var user = new UserEntity(firstName, lastName, userName, email);

            // Assert
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.UserName.Should().Be(userName);
            user.Email.Should().Be(email);
            
            user.Id.Should().NotBe(Guid.Empty);
            user.UserCode.Should().NotBeNullOrEmpty();
            user.UserCode.Length.Should().Be(7);
            
            // Collections should be initialized but empty
            user.Products.Should().NotBeNull().And.BeEmpty();
            user.UserRoles.Should().NotBeNull().And.BeEmpty();
            user.UserClaims.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void Constructor_WithNullLastName_ShouldSucceedAsItIsNullable()
        {
            // Arrange
            var firstName = "Jane";
            var userName = "janedoe";
            var email = "jane.doe@test.com";

            // Act
            var user = new UserEntity(firstName, null, userName, email);

            // Assert
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().BeNull();
            user.Should().NotBeNull();
        }

        #endregion

        #region Product Management Tests

        [Fact]
        public void AddProduct_WithValidProduct_ShouldAddProductToCollection()
        {
            // Arrange
            var user = CreateUserSut();
            var product = ProductEntity.Create(
                "Test Laptop", "A powerful laptop for developers", 1500, 10,
                ProductEntity.ProductState.Active, user.Id, Guid.NewGuid());

            // Act
            user.AddProduct(product);

            // Assert
            user.Products.Should().HaveCount(1);
            user.Products.First().Should().Be(product);
        }

        [Fact]
        public void AddProduct_WithNullProduct_ShouldThrowArgumentNullException()
        {
            // Arrange
            var user = CreateUserSut();

            // Act
            // The .Add method of List<T> throws this exception when a null item is added.
            Action act = () => user.AddProduct(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Products_Collection_ShouldBeReadOnlyToPreventExternalModification()
        {
            // Arrange
            var user = CreateUserSut();

            // Assert
            // This assertion checks that the publicly exposed collection cannot be cast
            // to a mutable list, enforcing encapsulation.
            user.Products.Should().NotBeAssignableTo<List<ProductEntity>>("because it should be a read-only view");
        }

        #endregion
    }
}