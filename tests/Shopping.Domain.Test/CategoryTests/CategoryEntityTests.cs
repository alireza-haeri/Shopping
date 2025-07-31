using FluentAssertions;
using Shopping.Domain.Entities.Product;

namespace Shoping.Domain.Test.CategoryTests
{
    /// <summary>
    /// Contains comprehensive unit tests for the CategoryEntity to ensure its
    /// construction, state changes, and business rules are correctly implemented.
    /// </summary>
    public class CategoryEntityTest
    {
        #region Test Setup

        /// <summary>
        /// Creates a valid CategoryEntity instance for use as the System Under Test (SUT).
        /// </summary>
        /// <param name="withParent">If true, creates a sub-category with a parent ID.</param>
        private CategoryEntity CreateCategorySut(bool withParent = false)
        {
            var parentId = withParent ? Guid.NewGuid() : (Guid?)null;
            return new CategoryEntity("Sample Category", parentId);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WhenCreatingRootCategory_ShouldSetParentIdToNull()
        {
            // Arrange
            var title = "Electronics";

            // Act
            var category = new CategoryEntity(title);

            // Assert
            category.Title.Should().Be(title);
            category.ParentId.Should().BeNull();
            category.Id.Should().NotBe(Guid.Empty);
            category.Products.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void Constructor_WhenCreatingSubCategory_ShouldSetParentId()
        {
            // Arrange
            var title = "Smartphones";
            var parentId = Guid.NewGuid();

            // Act
            var category = new CategoryEntity(title, parentId);

            // Assert
            category.Title.Should().Be(title);
            category.ParentId.Should().Be(parentId);
        }

        #endregion

        #region Edit Method Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Edit_WithNullOrEmptyTitle_ShouldThrowArgumentException(string invalidTitle)
        {
            // Arrange
            var category = CreateCategorySut();
            Action act = () => category.Edit(invalidTitle,null);

            // Assert
            act.Should().Throw<ArgumentException>()
                .And.ParamName.Should().Be("title");
        }

        [Fact]
        public void Edit_WhenCategoryIsSubCategory_ShouldUpdateTitleAndParentId()
        {
            // Arrange
            var category = CreateCategorySut(withParent: true); // A sub-category
            var newTitle = "Laptops & Computers";
            var newParentId = Guid.NewGuid();

            // Act
            category.Edit(newTitle, newParentId);

            // Assert
            category.Title.Should().Be(newTitle);
            category.ParentId.Should().Be(newParentId);
        }

        [Fact]
        public void Edit_WhenChangingSubCategoryToBeRoot_ShouldUpdateParentIdToNull()
        {
            // Arrange
            var category = CreateCategorySut(withParent: true); // A sub-category
            var newTitle = "All Products";

            // Act
            category.Edit(newTitle,null);

            // Assert
            category.Title.Should().Be(newTitle);
            category.ParentId.Should().BeNull();
        }

        #endregion

        #region Collection Encapsulation Tests

        [Fact]
        public void Products_Collection_ShouldBeReadOnlyToPreventExternalModification()
        {
            // Arrange
            var category = CreateCategorySut();

            // Assert
            // This assertion ensures the public property is a read-only wrapper
            // and not the underlying mutable list.
            category.Products.Should().NotBeAssignableTo<List<ProductEntity>>(
                "because external code should not be able to modify the collection directly");
        }

        #endregion
        
        #region Equality Tests

        [Fact]
        public void TwoCategories_WithDifferentIds_ShouldNotBeEqual()
        {
            // Arrange
            var category1 = CreateCategorySut();
            var category2 = CreateCategorySut();

            // Assert
            category1.Equals(category2).Should().BeFalse();
            (category1 != category2).Should().BeTrue();
        }

        #endregion
    }
}