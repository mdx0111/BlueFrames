using BlueFrames.Application.Common.Results;
using BlueFrames.Application.Orders.Queries.Common;
using BlueFrames.Application.Orders.Queries.GetCustomerOrder;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders;
using BlueFrames.Domain.Orders.Common;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.App.Tests.Unit.Orders;

public class GetOrderTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICustomerRepository _customerRepository = Substitute.For<ICustomerRepository>();
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();

    private readonly Product _product;
    private readonly Order _order;
    private readonly Customer _customer;

    private const int ProductSKUCharacterCount = 5;
    private const string ValidPhoneNumber = "07563385651";

    public GetOrderTests()
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
        
        _order = Order.Create(_product.Id, _customer.Id, OrderDate.From(_dateTimeService.UtcNow), _dateTimeService.UtcNow);
        _customer.PlaceOrder(_order);

        _dateTimeService.UtcNow.Returns(new DateTime(2025, 1, 1, 10, 25, 0));
    }
    
    [Fact]
    public async Task GetCustomerOrder_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var getOrder = new GetCustomerOrderQuery(
            _order.Id.Value,
            _customer.Id.Value);

        var logger = Substitute.For<ILoggerAdapter<GetCustomerOrderQueryHandler>>();
        var handler = new GetCustomerOrderQueryHandler(
            _customerRepository,
            logger);
        
        // Act
        var result = await handler.Handle(getOrder, _cancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<OrderDto>>();
        result.Value.Should().Be(OrderDto.From(_order));
    }
}