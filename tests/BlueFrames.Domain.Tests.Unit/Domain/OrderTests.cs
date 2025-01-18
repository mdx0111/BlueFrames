using BlueFrames.Domain.Orders;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class OrderTests
{
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
    private readonly Guid _productId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly DateTime _createdDate = new DateTime(2025, 01, 18, 14, 45, 0);

    public OrderTests()
    {
        _dateTimeService.UtcNow.Returns(_createdDate);
    }
    
    [Fact]
    public void CreateOrder_ShouldSucceed_WithValidData()
    {
        // Act
        var order = new Order(_productId, _customerId, _createdDate, _dateTimeService.UtcNow);

        // Assert
        order.Should().NotBeNull();
        order.ProductId.Should().Be(_productId);
        order.CustomerId.Should().Be(_customerId);
        order.CreatedDate.Should().Be(_createdDate);
        order.UpdatedDate.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public void CreateOrder_ShouldSuccess_WithPendingStatus()
    {
        // Act
        var order = new Order(_productId, _customerId, _createdDate, _dateTimeService.UtcNow);
        
        // Assert
        order.Status.Should().Be(Status.Pending);
    }
    
    [Fact]
    public void CreateOrder_ShouldFail_WhenProductIdIsInvalid()
    {
        // Arrange
        var invalidProductId = Guid.Empty;
        
        // Act
        Action createOrder = () => _ =new Order(invalidProductId, _customerId, _createdDate, _dateTimeService.UtcNow);
        
        // Assert
        createOrder.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void CreateOrder_ShouldFail_WhenCustomerIdIsInvalid()
    {
        // Arrange
        var invalidCustomerId = Guid.Empty;
        
        // Act
        Action createOrder = () => _ =new Order(_productId, invalidCustomerId, _createdDate, _dateTimeService.UtcNow);
        
        // Assert
        createOrder.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void CreateOrder_ShouldFail_WhenCreatedDateIsInvalid()
    {
        // Arrange
        var invalidCreatedDate = new DateTime(2020, 01, 18, 14, 45, 0);
        
        // Act
        Action createOrder = () => _ =new Order(_productId, _customerId, invalidCreatedDate, _dateTimeService.UtcNow);
        
        // Assert
        createOrder.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void UpdateStatus_ShouldSuccess_WhenStatusIsValid()
    {
        // Arrange
        var order = new Order(_productId, _customerId, _createdDate, _dateTimeService.UtcNow);
        var newStatus = Status.Complete;
        var updatedDate = new DateTime(2025, 01, 18, 15, 45, 0);
        _dateTimeService.UtcNow.Returns(updatedDate);
        
        // Act
        order.UpdateStatus(newStatus, _dateTimeService.UtcNow);
        
        // Assert
        order.Status.Should().Be(newStatus);
        order.UpdatedDate.Should().Be(updatedDate);
    }
    
    [Fact]
    public void UpdateStatus_ShouldFail_WhenStatusIsInvalid()
    {
        // Arrange
        var order = new Order(_productId, _customerId, _createdDate, _dateTimeService.UtcNow);
        var newStatus = Status.Pending;
        
        // Act
        Action updateStatus = () => order.UpdateStatus(newStatus, _dateTimeService.UtcNow);
        
        // Assert
        updateStatus.Should().Throw<ValidationException>();
    }
}