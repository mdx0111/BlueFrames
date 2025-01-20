using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders;
using BlueFrames.Domain.Orders.Common;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class OrderTests
{
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
    private readonly ProductId _productId = ProductId.From(Guid.NewGuid());
    private readonly CustomerId _customerId = CustomerId.From(Guid.NewGuid());
    private readonly OrderDate _createdDate = OrderDate.From(new DateTime(2025, 01, 18, 14, 45, 0));

    public OrderTests()
    {
        _dateTimeService.UtcNow.Returns(_createdDate.Value);
    }
    
    [Fact]
    public void CreateOrder_ShouldSucceed_WithValidData()
    {
        // Act
        var order = Order.Create(_productId, _customerId, _createdDate);

        // Assert
        order.Should().NotBeNull();
        order.ProductId.Should().Be(_productId);
        order.CustomerId.Should().Be(_customerId);
        order.CreatedDate.Should().BeEquivalentTo(_createdDate);
        order.UpdatedDate.Should().BeNull();
    }

    [Fact]
    public void CreateOrder_ShouldCreateOrder_WithValidId()
    {
        // Act
        var order = Order.Create(_productId, _customerId, _createdDate);
        
        // Assert
        order.Id.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void CreateOrder_ShouldSuccess_WithPendingStatus()
    {
        // Act
        var order = Order.Create(_productId, _customerId, _createdDate);
        
        // Assert
        order.Status.Should().Be(Status.Pending);
    }
    
    [Fact]
    public void CreateOrder_ShouldReturnFailure_WhenProductIdIsInvalid()
    {
        // Act
        Action createOrder = () => _ =Order.Create(ProductId.From(Guid.Empty), _customerId, _createdDate);
        
        // Assert
        createOrder.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void CreateOrder_ShouldReturnFailure_WhenCustomerIdIsInvalid()
    {
        // Act
        Action createOrder = () => _ =Order.Create(_productId, CustomerId.From(Guid.Empty), _createdDate);
        
        // Assert
        createOrder.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Cancel_ShouldSuccess_WhenOrderIsNotCancelled()
    {
        // Arrange
        var order = Order.Create(_productId, _customerId, _createdDate);
        var updatedDate = new DateTime(2025, 01, 18, 15, 45, 0);
        _dateTimeService.UtcNow.Returns(updatedDate);
        
        // Act
        order.Cancel(_dateTimeService.UtcNow);
        
        // Assert
        order.Status.Should().Be(Status.Cancelled);
        order.UpdatedDate.Value.Should().Be(updatedDate);
    }
    
    [Fact]
    public void Cancel_ShouldReturnFailure_WhenOrderIsAlreadyCancelled()
    {
        // Arrange
        var order = Order.Create(_productId, _customerId, _createdDate);
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
        var order = Order.Create(_productId, _customerId, _createdDate);
        var updatedDate = new DateTime(2025, 01, 18, 15, 45, 0);
        _dateTimeService.UtcNow.Returns(updatedDate);
        
        // Act
        order.Complete(_dateTimeService.UtcNow);
        
        // Assert
        order.Status.Should().Be(Status.Complete);
        order.UpdatedDate.Value.Should().Be(updatedDate);
    }
    
    [Fact]
    public void Complete_ShouldReturnFailure_WhenOrderIsAlreadyCompleted()
    {
        // Arrange
        var order = Order.Create(_productId, _customerId, _createdDate);
        order.Complete(_dateTimeService.UtcNow);
        
        // Act
        Action complete = () => order.Complete(_dateTimeService.UtcNow);
        
        // Assert
        complete.Should().Throw<ValidationException>();
    }
}