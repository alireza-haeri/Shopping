using FluentAssertions;
using Shopping.Domain.Entities.Cart;
using System;
using Xunit;

namespace Shoping.Domain.Test.CartTests;

/// <summary>
/// Contains focused unit tests for the CartItemEntity to ensure its internal
/// business rules and state changes are correctly implemented.
/// </summary>
public class CartItemEntityTest
{
    #region Creation Tests

    [Fact]
    public void Create_WithValidParameters_ShouldCreateCartItemCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var quantity = 2;
        var unitPrice = 10.00m;

        // Act
        var cartItem = CartItemEntity.Create(productId, quantity, unitPrice);

        // Assert
        cartItem.Should().NotBeNull();
        cartItem.Id.Should().NotBe(Guid.Empty);
        cartItem.ProductId.Should().Be(productId);
        cartItem.Quantity.Should().Be(quantity);
        cartItem.UnitPrice.Should().Be(unitPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidQuantity_ShouldThrowArgumentException(int invalidQuantity)
    {
        // Arrange
        Action act = () => CartItemEntity.Create(Guid.NewGuid(), invalidQuantity, 100m);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50.5)]
    public void Create_WithInvalidUnitPrice_ShouldThrowArgumentException(double invalidPrice)
    {
        // Arrange
        Action act = () => CartItemEntity.Create(Guid.NewGuid(), 1, (decimal)invalidPrice);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyProductId_ShouldThrowArgumentException()
    {
        // Arrange
        Action act = () => CartItemEntity.Create(Guid.Empty, 1, 100m);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Behavior Tests

    [Fact]
    public void IncreaseQuantity_WithPositiveValue_ShouldUpdateQuantity()
    {
        // Arrange
        var cartItem = CartItemEntity.Create(Guid.NewGuid(), 2, 100m);
        var quantityToAdd = 3;

        // Act
        cartItem.IncreaseQuantity(quantityToAdd);

        // Assert
        cartItem.Quantity.Should().Be(5);
    }

    [Fact]
    public void IncreaseQuantity_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var cartItem = CartItemEntity.Create(Guid.NewGuid(), 2, 100m);
        Action act = () => cartItem.IncreaseQuantity(-1);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetQuantity_WithPositiveValue_ShouldOverwriteQuantity()
    {
        // Arrange
        var cartItem = CartItemEntity.Create(Guid.NewGuid(), 1, 100m);
        var newQuantity = 5;

        // Act
        cartItem.SetQuantity(newQuantity);

        // Assert
        cartItem.Quantity.Should().Be(newQuantity);
    }

    [Fact]
    public void SetQuantity_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var cartItem = CartItemEntity.Create(Guid.NewGuid(), 1, 100m);
        Action act = () => cartItem.SetQuantity(-5);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion
}