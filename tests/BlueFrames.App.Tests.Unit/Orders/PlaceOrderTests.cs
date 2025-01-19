using BlueFrames.Application.Orders.Commands.PlaceOrder;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.App.Tests.Unit.Orders;

public class PlaceOrderTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICustomerRepository _customerRepository = Substitute.For<ICustomerRepository>();
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILoggerAdapter<PlaceOrderCommandHandler> _logger = Substitute.For<ILoggerAdapter<PlaceOrderCommandHandler>>();

    private readonly Product _product;
    private readonly Customer _customer;

    private const int ProductSKUCharacterCount = 5;
    private const string ValidPhoneNumber = "07563385651";

    public PlaceOrderTests()
    {
        var commerce = new Bogus.DataSets.Commerce();
        _product = Product.Create(
            ProductName.From(commerce.ProductName()),
            ProductDescription.From(commerce.ProductDescription()),
            ProductSku.From(commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()));
        _productRepository.GetByIdAsync(_product.Id.Value, _cancellationToken).Returns(_product);

        var person = new Bogus.Person(locale: "en_GB");
        _customer = Customer.Create(
            FirstName.From(person.FirstName),
            LastName.From(person.LastName),
            PhoneNumber.From(ValidPhoneNumber),
            Email.From(person.Email));
        _customerRepository.GetByIdAsync(_customer.Id.Value, _cancellationToken).Returns(_customer);
        
        _dateTimeService.UtcNow.Returns(new DateTime(2025, 1, 1, 10, 25, 0));
    }
    
    [Fact]
    public async Task PlaceOrder_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var placeOrder = new PlaceOrderCommand(
            _customer.Id.Value,
            _product.Id.Value);

        var handler = new PlaceOrderCommandHandler(
            _customerRepository,
            _productRepository,
            _dateTimeService,
            _unitOfWork,
            _logger);
        
        // Act
        var result = await handler.Handle(placeOrder, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var createdOrderId = result.Value;
        _customer.HasOrder(createdOrderId).Should().BeTrue();
    }
    
    [Fact]
    public async Task PlaceOrder_ShouldFail_WhenCustomerNotFound()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(0);

        var placeOrder = new PlaceOrderCommand(
            Guid.Empty, 
            _product.Id.Value);

        var handler = new PlaceOrderCommandHandler(
            _customerRepository,
            _productRepository,
            _dateTimeService,
            _unitOfWork,
            _logger);

        // Act
        var result = await handler.Handle(placeOrder, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
    
    [Fact]
    public async Task PlaceOrder_ShouldFail_WhenProductNotFound()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(0);

        var placeOrder = new PlaceOrderCommand(
            _customer.Id.Value, 
            Guid.Empty);

        var handler = new PlaceOrderCommandHandler(
            _customerRepository,
            _productRepository,
            _dateTimeService,
            _unitOfWork,
            _logger);

        // Act
        var result = await handler.Handle(placeOrder, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}