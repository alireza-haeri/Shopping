using FluentAssertions;
using Shopping.Domain.Common.ValueObjects;
using Shopping.Domain.Entities.Product;

namespace Shoping.Domain.Test.ProductTests
{
    /// <summary>
    /// Contains comprehensive unit tests for the ProductEntity to ensure
    /// business logic, validation, and state changes are handled correctly.
    /// </summary>
    public class ProductEntityTests
    {
        #region Test Setup

        /// <summary>
        /// Creates a valid ProductEntity instance to be used as the System Under Test (SUT).
        /// This avoids code repetition in test arrangements.
        /// </summary>
        private ProductEntity CreateValidProductSut()
        {
            return ProductEntity.Create(
                id: Guid.NewGuid(),
                title: "Valid Product Title",
                description: "A valid and interesting product description.",
                price: 150.75m,
                quantity: 10,
                state: ProductEntity.ProductState.Active,
                userId: Guid.NewGuid(),
                categoryId: Guid.NewGuid());
        }

        #endregion

        #region Creation Tests

        [Fact]
        public void Create_WithValidParameters_ShouldSucceedAndInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var title = "Test Product";
            var description = "Test Description";
            var price = 100m;
            var quantity = 5;
            var state = ProductEntity.ProductState.ShowOnly;
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            // Act
            var product = ProductEntity.Create(id, title, description, price, quantity, state, userId, categoryId);

            // Assert
            product.Id.Should().Be(id);
            product.Title.Should().Be(title);
            product.Description.Should().Be(description);
            product.Price.Should().Be(price);
            product.Quantity.Should().Be(quantity);
            product.State.Should().Be(state);
            product.UserId.Should().Be(userId);
            product.CategoryId.Should().Be(categoryId);
            product.Images.Should().BeEmpty();
            product.ChangeLogs.Should().HaveCount(1);
            product.ChangeLogs.First().Message.Should().Be("Product Created");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Create_WithNullOrEmptyTitle_ShouldThrowArgumentException(string? invalidTitle)
        {
            // Arrange
            Action act = () => ProductEntity.Create(invalidTitle, "Desc", 100, 1, ProductEntity.ProductState.Active, Guid.NewGuid(), null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Invalid Title*");
        }
        
        [Fact]
        public void Create_WithEmptyId_ShouldThrowArgumentException()
        {
            // Arrange
            Action act = () => ProductEntity.Create(Guid.Empty, "Title", "Desc", 100, 1, ProductEntity.ProductState.Active, Guid.NewGuid(), null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Invalid Id*");
        }

        [Fact]
        public void Create_WithEmptyUserId_ShouldThrowArgumentException()
        {
            // Arrange
            Action act = () => ProductEntity.Create("Title", "Desc", 100, 1, ProductEntity.ProductState.Active, Guid.Empty, null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Invalid User Id*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50.5)]
        public void Create_WithZeroOrNegativePrice_ShouldThrowArgumentException(decimal invalidPrice)
        {
            // Arrange
            Action act = () => ProductEntity.Create("Title", "Desc", invalidPrice, 1, ProductEntity.ProductState.Active, Guid.NewGuid(), null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Invalid Price*");
        }

        [Fact]
        public void Create_WithNegativeQuantity_ShouldThrowArgumentException()
        {
            // Arrange
            Action act = () => ProductEntity.Create("Title", "Desc", 100, -1, ProductEntity.ProductState.Active, Guid.NewGuid(), null);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Invalid Quantity*");
        }

        #endregion

        #region Edit Tests

        [Fact]
        public void Edit_WithValidParameters_ShouldUpdatePropertiesAndAddLog()
        {
            // Arrange
            var product = CreateValidProductSut();
            var logCountBeforeEdit = product.ChangeLogs.Count;
            
            var newTitle = "Updated Title";
            var newDescription = "Updated Description";
            var newPrice = 299.99m;
            var newQuantity = 20;
            var newCategoryId = Guid.NewGuid();

            // Act
            product.Edit(newTitle, newDescription, newPrice, newQuantity, newCategoryId);

            // Assert
            product.Title.Should().Be(newTitle);
            product.Description.Should().Be(newDescription);
            product.Price.Should().Be(newPrice);
            product.Quantity.Should().Be(newQuantity);
            product.CategoryId.Should().Be(newCategoryId);
            
            product.ChangeLogs.Should().HaveCount(logCountBeforeEdit + 1);
            product.ChangeLogs.Last().Message.Should().Be("Product Edited");
        }

        [Fact]
        public void Edit_WithNullOrEmptyParameters_ShouldOnlyUpdateProvidedValues()
        {
            // Arrange
            var product = CreateValidProductSut();
            var originalDescription = product.Description;
            var originalPrice = product.Price;
            var newTitle = "A New Title Is Here";

            // Act - Only provide a new title, other parameters are null.
            product.Edit(newTitle, null, null, null, null);

            // Assert
            product.Title.Should().Be(newTitle); // This should change.
            product.Description.Should().Be(originalDescription); // This should NOT change.
            product.Price.Should().Be(originalPrice); // This should NOT change.
        }

        #endregion

        #region State Change Tests

        [Fact]
        public void ChangeState_ShouldUpdateStateAndAddLogWithDescription()
        {
            // Arrange
            var product = CreateValidProductSut();
            var logCountBeforeChange = product.ChangeLogs.Count;
            var newState = ProductEntity.ProductState.Hidden;
            var reason = "Item is out of season.";

            // Act
            product.ChangeState(newState, reason);

            // Assert
            product.State.Should().Be(newState);
            product.ChangeLogs.Should().HaveCount(logCountBeforeChange + 1);
            product.ChangeLogs.Last().Message.Should().Be("Product State Changed");
            product.ChangeLogs.Last().AdditionalDescription.Should().Be(reason);
        }

        #endregion

        #region Image Management Tests

        [Fact]
        public void AddImage_WithValidImage_ShouldAddImageToCollection()
        {
            // Arrange
            var product = CreateValidProductSut();
            var image = new ImageValueObject("new-image.jpg", "image/jpeg", "A new shiny image");
            
            // Act
            product.AddImage(image);

            // Assert
            product.Images.Should().HaveCount(1);
            product.Images.First().Should().Be(image);
        }

        [Fact]
        public void AddImage_WithNullImage_ShouldThrowArgumentNullException()
        {
            // Arrange
            var product = CreateValidProductSut();

            // Act
            Action act = () => product.AddImage(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveImages_WithExistingImageNames_ShouldRemoveThemFromCollection()
        {
            // Arrange
            var product = CreateValidProductSut();
            var image1 = new ImageValueObject("image-to-keep.jpg", "image/jpeg", "Keep");
            var image2 = new ImageValueObject("image-to-remove.png", "image/png", "Remove");
            product.AddImage(image1);
            product.AddImage(image2);

            // Act
            product.RemoveImages(new[] { "image-to-remove.png" });

            // Assert
            product.Images.Should().HaveCount(1);
            product.Images.First().FileName.Should().Be("image-to-keep.jpg");
        }
        
        [Fact]
        public void RemoveImages_WithNonExistentName_ShouldNotChangeCollection()
        {
            // Arrange
            var product = CreateValidProductSut();
            var image1 = new ImageValueObject("image1.jpg", "image/jpeg", "Img1");
            product.AddImage(image1);

            // Act
            product.RemoveImages(new[] { "non-existent-image.gif" });

            // Assert
            product.Images.Should().HaveCount(1);
        }
        
        [Fact]
        public void RemoveImages_WithEmptyArray_ShouldNotChangeCollection()
        {
            // Arrange
            var product = CreateValidProductSut();
            product.AddImage(new ImageValueObject("image1.jpg", "image/jpeg", "Img1"));

            // Act
            product.RemoveImages(Array.Empty<string>());

            // Assert
            product.Images.Should().HaveCount(1);
        }
        
        [Fact]
        public void RemoveImages_WithNullArray_ShouldThrowArgumentNullException()
        {
            // Arrange
            var product = CreateValidProductSut();

            // Act
            Action act = () => product.RemoveImages(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void TwoProducts_WithSameId_ShouldBeEqual()
        {
            // Arrange
            var id = Guid.NewGuid();
            var product1 = ProductEntity.Create(id, "Title", "Desc", 1, 1, ProductEntity.ProductState.Active, Guid.NewGuid(), null);
            var product2 = ProductEntity.Create(id, "OtherTitle", "OtherDesc", 2, 2, ProductEntity.ProductState.Hidden, Guid.NewGuid(), null);

            // Act & Assert
            product1.Equals(product2).Should().BeTrue();
            (product1 == product2).Should().BeTrue();
            (product1 != product2).Should().BeFalse();
        }

        [Fact]
        public void TwoProducts_WithDifferentIds_ShouldNotBeEqual()
        {
            // Arrange
            var product1 = CreateValidProductSut();
            var product2 = CreateValidProductSut(); // Will have a different GUID

            // Act & Assert
            product1.Equals(product2).Should().BeFalse();
            (product1 == product2).Should().BeFalse();
            (product1 != product2).Should().BeTrue();
        }

        #endregion
    }
}