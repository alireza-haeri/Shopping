using FluentAssertions;
using Shopping.Domain.Entities.Cart;
using System;
using System.Linq;
using Xunit;

namespace Shoping.Domain.Test.CartTests;

/// <summary>
/// Contains comprehensive unit tests for the CartEntity and its aggregate, CartItemEntity,
/// to ensure construction, state changes, and business rules are correctly implemented.
/// </summary>
public class CartEntityTest
{
    #region Test Setup

    /// <summary>
    /// Creates a valid, empty CartEntity instance for a given user to act as the System Under Test (SUT).
    /// </summary>
    private CartEntity CreateCartSut()
    {
        var userId = Guid.NewGuid();
        return CartEntity.Create(userId);
    }

    #endregion

    #region Creation (Constructor) Tests

    [Fact]
    public void Create_WithValidUserId_ShouldCreateEmptyCart()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var cart = CartEntity.Create(userId);

        // Assert
        cart.Should().NotBeNull();
        cart.Id.Should().NotBe(Guid.Empty);
        cart.UserId.Should().Be(userId);
        cart.Items.Should().NotBeNull().And.BeEmpty();
        cart.TotalPrice.Should().Be(0);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange
        Action act = () => CartEntity.Create(Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region AddItem Method Tests

    [Fact]
    public void AddItem_WhenProductIsNotInCart_ShouldAddNewItem()
    {
        // Arrange
        var cart = CreateCartSut();
        var productId = Guid.NewGuid();
        var quantity = 2;
        var unitPrice = 150.50m;

        // Act
        cart.AddItem(productId, quantity, unitPrice);

        // Assert
        cart.Items.Should().HaveCount(1);
        var addedItem = cart.Items.Single();
        addedItem.ProductId.Should().Be(productId);
        addedItem.Quantity.Should().Be(quantity);
        addedItem.UnitPrice.Should().Be(unitPrice);
        cart.TotalPrice.Should().Be(quantity * unitPrice);
    }

    [Fact]
    public void AddItem_WhenProductIsAlreadyInCart_ShouldIncreaseQuantityOfExistingItem()
    {
        // Arrange
        var cart = CreateCartSut();
        var productId = Guid.NewGuid();
        cart.AddItem(productId, 1, 100m); // Add initial item

        // Act
        cart.AddItem(productId, 2, 100m); // Add more of the same item

        // Assert
        cart.Items.Should().HaveCount(1, "because it should update the existing item, not add a new one");
        cart.Items.Single().Quantity.Should().Be(3);
        cart.TotalPrice.Should().Be(300m);
    }

    [Theory]
    [InlineData(0, 100)] // Invalid quantity
    [InlineData(-1, 100)] // Invalid quantity
    [InlineData(1, 0)] // Invalid price
    [InlineData(1, -50)] // Invalid price
    public void AddItem_WithInvalidParameters_ShouldThrowArgumentException(int quantity, decimal unitPrice)
    {
        // Arrange
        var cart = CreateCartSut();
        var productId = Guid.NewGuid();
        Action act = () => cart.AddItem(productId, quantity, unitPrice);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region RemoveItem Method Tests

    [Fact]
    public void RemoveItem_WithExistingItemId_ShouldRemoveTheItemFromCart()
    {
        // Arrange
        var cart = CreateCartSut();
        var product1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        cart.AddItem(product1Id, 1, 100m);
        cart.AddItem(product2Id, 2, 50m);
        var itemToRemove = cart.Items.First(i => i.ProductId == product1Id);
        var initialTotalPrice = cart.TotalPrice; // 100 + 100 = 200

        // Act
        cart.RemoveItem(itemToRemove.Id);

        // Assert
        cart.Items.Should().HaveCount(1);
        cart.Items.Single().ProductId.Should().Be(product2Id);
        cart.TotalPrice.Should().Be(100m, $"because the item worth {initialTotalPrice - 100m} was removed");
    }

    [Fact]
    public void RemoveItem_WithNonExistingItemId_ShouldDoNothing()
    {
        // Arrange
        var cart = CreateCartSut();
        cart.AddItem(Guid.NewGuid(), 1, 100m);
        var initialCount = cart.Items.Count;
        var initialPrice = cart.TotalPrice;
        var nonExistingItemId = Guid.NewGuid();

        // Act
        Action act = () => cart.RemoveItem(nonExistingItemId);

        // Assert
        act.Should().NotThrow();
        cart.Items.Should().HaveCount(initialCount);
        cart.TotalPrice.Should().Be(initialPrice);
    }

    #endregion

    #region UpdateItemQuantity Method Tests

    [Fact]
    public void UpdateItemQuantity_WithExistingItem_ShouldSetNewQuantity()
    {
        // Arrange
        var cart = CreateCartSut();
        cart.AddItem(Guid.NewGuid(), 1, 100m);
        var itemToUpdate = cart.Items.Single();
        var newQuantity = 5;

        // Act
        cart.UpdateItemQuantity(itemToUpdate.Id, newQuantity);

        // Assert
        itemToUpdate.Quantity.Should().Be(newQuantity);
        cart.TotalPrice.Should().Be(500m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void UpdateItemQuantity_WithInvalidQuantity_ShouldThrowArgumentException(int invalidQuantity)
    {
        // Arrange
        var cart = CreateCartSut();
        cart.AddItem(Guid.NewGuid(), 1, 100m);
        var itemToUpdate = cart.Items.Single();
        Action act = () => cart.UpdateItemQuantity(itemToUpdate.Id, invalidQuantity);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Clear Method Tests

    [Fact]
    public void Clear_WhenCartHasItems_ShouldRemoveAllItems()
    {
        // Arrange
        var cart = CreateCartSut();
        cart.AddItem(Guid.NewGuid(), 1, 100m);
        cart.AddItem(Guid.NewGuid(), 2, 50m);

        // Act
        cart.Clear();

        // Assert
        cart.Items.Should().BeEmpty();
        cart.TotalPrice.Should().Be(0);
    }

    #endregion

    #region Collection Encapsulation Tests

    [Fact]
    public void Items_Collection_ShouldBeReadOnlyToPreventExternalModification()
    {
        // Arrange
        var cart = CreateCartSut();

        // Assert
        // This assertion ensures the public 'Items' property is a read-only wrapper
        // and not the underlying mutable list.
        cart.Items.Should().NotBeAssignableTo<List<CartItemEntity>>(
            "because external code should not be able to modify the collection directly");
    }

    #endregion
}