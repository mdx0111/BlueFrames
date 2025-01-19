using BlueFrames.Application.Orders.Commands.CompleteOrder;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders;
using BlueFrames.Domain.Orders.Common;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.App.Tests.Unit.Orders;

public class CompleteOrderTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ICustomerRepository _customerRepository = Substitute.For<ICustomerRepository>();
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILoggerAdapter<CompleteOrderCommandHandler> _logger = Substitute.For<ILoggerAdapter<CompleteOrderCommandHandler>>();

    private readonly Order _order;
    private readonly Customer _customer;

    private const int ProductSKUCharacterCount = 5;
    private const string ValidPhoneNumber = "07563385651";

    public CompleteOrderTests()
    {
        _dateTimeService.UtcNow.Returns(new DateTime(2025, 1, 1, 10, 25, 0));
        
        var commerce = new Bogus.DataSets.Commerce();
        var product = Product.Create(
            ProductName.From(commerce.ProductName()),
            ProductDescription.From(commerce.ProductDescription()),
            ProductSKU.From(commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()));
        
        var person = new Bogus.Person(locale: "en_GB");
        _customer = Customer.Create(
            FirstName.From(person.FirstName),
            LastName.From(person.LastName),
            PhoneNumber.From(ValidPhoneNumber),
            Email.From(person.Email));
        _customerRepository.GetByIdAsync(_customer.Id.Value, _cancellationToken).Returns(_customer);
        
        _order = Order.Create(product.Id, _customer.Id, OrderDate.From(_dateTimeService.UtcNow), _dateTimeService.UtcNow);
        _customer.PlaceOrder(_order);
    }
    
    [Fact]
    public async Task CompleteOrder_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var completeOrder = new CompleteOrderCommand(
            _order.Id,
            _customer.Id);

        var handler = new CompleteOrderCommandHandler(
            _customerRepository,
            _dateTimeService,
            _unitOfWork,
            _logger);
        
        // Act
        var result = await handler.Handle(completeOrder, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task CompleteOrder_ShouldReturnFailure_WhenOrderNotFound()
    {
        // Arrange
        var completeOrder = new CompleteOrderCommand(
            OrderId.From(Guid.NewGuid()),
            _customer.Id);

        var handler = new CompleteOrderCommandHandler(
            _customerRepository,
            _dateTimeService,
            _unitOfWork,
            _logger);
        
        // Act
        var result = await handler.Handle(completeOrder, _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public async Task CompleteOrder_ShouldReturnFailure_WhenCustomerNotFound()
    {
        // Arrange
        var completeOrder = new CompleteOrderCommand(
            _order.Id,
            CustomerId.From(Guid.NewGuid()));

        var handler = new CompleteOrderCommandHandler(
            _customerRepository,
            _dateTimeService,
            _unitOfWork,
            _logger);
        
        // Act
        var result = await handler.Handle(completeOrder, _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}