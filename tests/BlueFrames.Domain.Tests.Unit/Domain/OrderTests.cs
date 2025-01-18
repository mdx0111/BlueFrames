using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class OrderTests
{
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
    private readonly ProductId _productId = ProductId.From(Guid.NewGuid());
    private readonly CustomerId _customerId = CustomerId.From(Guid.NewGuid());
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
    public void CreateOrder_ShouldCreateOrder_WithValidId()
    {
        // Act
        var order = new Order(_productId, _customerId, _createdDate, _dateTimeService.UtcNow);
        
        // Assert
        order.Id.Value.Should().NotBeEmpty();
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
        // Act
        Action createOrder = () => _ =new Order(ProductId.From(Guid.Empty), _customerId, _createdDate, _dateTimeService.UtcNow);
        
        // Assert
        createOrder.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void CreateOrder_ShouldFail_WhenCustomerIdIsInvalid()
    {
        // Act
        Action createOrder = () => _ =new Order(_productId, CustomerId.From(Guid.Empty), _createdDate, _dateTimeService.UtcNow);
        
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
    public void Cancel_ShouldSuccess_WhenOrderIsNotCancelled()
    {
        // Arrange
        var order = new Order(_productId, _customerId, _createdDate, _dateTimeService.UtcNow);
        var updatedDate = new DateTime(2025, 01, 18, 15, 45, 0);
        _dateTimeService.UtcNow.Returns(updatedDate);
        
        // Act
        order.Cancel(_dateTimeService.UtcNow);
        
        // Assert
        order.Status.Should().Be(Status.Cancelled);
        order.UpdatedDate.Should().Be(updatedDate);
    }
    
    [Fact]
    public void Cancel_ShouldFail_WhenOrderIsAlreadyCancelled()
    {
        // Arrange
        var order = new Order(_productId, _customerId, _createdDate, _dateTimeService.UtcNow);
        order.Cancel(_dateTimeService.UtcNow);
        
        // Act
        Action cancel = () => order.Cancel(_dateTimeService.UtcNow);
        
        // Assert
        cancel.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Complete_ShouldSuccess_WhenOrderIsNotCompleted()
    {
        // Arrange
        var order = new Order(_productId, _customerId, _createdDate, _dateTimeService.UtcNow);
        var updatedDate = new DateTime(2025, 01, 18, 15, 45, 0);
        _dateTimeService.UtcNow.Returns(updatedDate);
        
        // Act
        order.Complete(_dateTimeService.UtcNow);
        
        // Assert
        order.Status.Should().Be(Status.Complete);
        order.UpdatedDate.Should().Be(updatedDate);
    }
    
    [Fact]
    public void Complete_ShouldFail_WhenOrderIsAlreadyCompleted()
    {
        // Arrange
        var order = new Order(_productId, _customerId, _createdDate, _dateTimeService.UtcNow);
        order.Complete(_dateTimeService.UtcNow);
        
        // Act
        Action complete = () => order.Complete(_dateTimeService.UtcNow);
        
        // Assert
        complete.Should().Throw<ValidationException>();
    }
}