using BlueFrames.Domain.Orders;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class OrderTests
{
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
    
    [Fact]
    public void CreateOrder_ShouldSucceed_WithValidData()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var createdDate = new DateTime(2025, 01, 18, 14, 45, 0);
        
        _dateTimeService.UtcNow.Returns(createdDate);
        
        // Act
        var order = new Order(productId, customerId, createdDate, _dateTimeService.UtcNow);

        // Assert
        order.Should().NotBeNull();
        order.ProductId.Should().Be(productId);
        order.CustomerId.Should().Be(customerId);
        order.CreatedDate.Should().Be(createdDate);
        order.UpdatedDate.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void CreateOrder_ShouldSuccess_WithPendingStatus()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var createdDate = new DateTime(2025, 01, 18, 14, 45, 0);
        
        _dateTimeService.UtcNow.Returns(createdDate);

        // Act
        var order = new Order(productId, customerId, createdDate, _dateTimeService.UtcNow);
        
        // Assert
        order.Status.Should().Be(Status.Pending);
    }
    
    [Fact]
    public void CreateOrder_ShouldFail_WhenProductIdIsInvalid()
    {
        // Arrange
        var productId = Guid.Empty;
        var customerId = Guid.NewGuid();
        var createdDate = new DateTime(2025, 01, 18, 14, 45, 0);
       
        _dateTimeService.UtcNow.Returns(createdDate);

        // Act
        Action createOrder = () => _ =new Order(productId, customerId, createdDate, _dateTimeService.UtcNow);
        
        // Assert
        createOrder.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void CreateOrder_ShouldFail_WhenCustomerIdIsInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var customerId = Guid.Empty;
        var createdDate = new DateTime(2025, 01, 18, 14, 45, 0);

        _dateTimeService.UtcNow.Returns(createdDate);

        // Act
        Action createOrder = () => _ =new Order(productId, customerId, createdDate, _dateTimeService.UtcNow);
        
        // Assert
        createOrder.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void CreateOrder_ShouldFail_WhenCreatedDateIsInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var createdDate = DateTime.MinValue;

        _dateTimeService.UtcNow.Returns(new DateTime(2025, 01, 18, 14, 45, 0));

        // Act
        Action createOrder = () => _ =new Order(productId, customerId, createdDate, _dateTimeService.UtcNow);
        
        // Assert
        createOrder.Should().Throw<ValidationException>();
    }
}