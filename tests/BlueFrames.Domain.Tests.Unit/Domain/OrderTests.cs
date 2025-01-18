using BlueFrames.Domain.Orders;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class OrderTests
{
    [Fact]
    public void CreateOrder_ShouldSucceed_WithValidData()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var createdDate = new DateTime(2025, 01, 18, 14, 45, 0);
        
        // Act
        var order = new Order(productId, customerId, createdDate);

        // Assert
        order.Should().NotBeNull();
        order.ProductId.Should().Be(productId);
        order.CustomerId.Should().Be(customerId);
        order.CreatedDate.Should().Be(createdDate);
        order.UpdatedDate.Should().Be(DateTime.MinValue);
    }
}