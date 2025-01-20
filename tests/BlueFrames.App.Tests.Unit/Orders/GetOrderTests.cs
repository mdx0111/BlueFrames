using BlueFrames.Application.Common.Results;
using BlueFrames.Application.Orders.Queries.Common;
using BlueFrames.Application.Orders.Queries.GetCustomerOrder;
using BlueFrames.Application.Orders.Queries.GetCustomerOrderDetails;
using BlueFrames.Application.Orders.Queries.GetCustomerOrders;
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

    private readonly Order _order;
    private readonly Customer _customer;

    private const int ProductSKUCharacterCount = 5;

    public GetOrderTests()
    {
        var commerce = new Bogus.DataSets.Commerce();
        var product = Product.Create(
            ProductName.From(commerce.ProductName()),
            ProductDescription.From(commerce.ProductDescription()),
            ProductSKU.From(commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()));
        _productRepository.GetByIdAsync(product.Id, _cancellationToken).Returns(product);

        var faker = new Bogus.Faker("en_GB");
        var person = new Bogus.Person(locale: "en_GB");
        _customer = Customer.Create(
            FirstName.From(person.FirstName),
            LastName.From(person.LastName),
            PhoneNumber.From(faker.Phone.PhoneNumberFormat(1)),
            Email.From(person.Email));
        _customerRepository.GetByIdAsync(_customer.Id, _cancellationToken).Returns(_customer);
        
        _order = Order.Create(product.Id, _customer.Id, OrderDate.From(_dateTimeService.UtcNow));
        _customer.PlaceOrder(_order);

        _dateTimeService.UtcNow.Returns(new DateTime(2025, 1, 1, 10, 25, 0));
    }
    
    [Fact]
    public async Task GetCustomerOrder_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var getOrder = new GetCustomerOrderQuery(
            _order.Id,
            _customer.Id);

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
    
    [Fact]
    public async Task GetCustomerOrder_ShouldReturnEmpty_WhenOrderDoesNotExist()
    {
        // Arrange
        var getOrder = new GetCustomerOrderQuery(
            OrderId.From(Guid.NewGuid()),
            _customer.Id);

        var logger = Substitute.For<ILoggerAdapter<GetCustomerOrderQueryHandler>>();
        var handler = new GetCustomerOrderQueryHandler(
            _customerRepository,
            logger);
        
        // Act
        var result = await handler.Handle(getOrder, _cancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<OrderDto>>();
        result.Value.Should().BeNull();
    }
    
    [Fact]
    public async Task GetCustomerOrder_ShouldReturnFailure_WhenCustomerDoesNotExist()
    {
        // Arrange
        var getOrder = new GetCustomerOrderQuery(
            _order.Id,
            CustomerId.From(Guid.NewGuid()));

        var logger = Substitute.For<ILoggerAdapter<GetCustomerOrderQueryHandler>>();
        var handler = new GetCustomerOrderQueryHandler(
            _customerRepository,
            logger);
        
        // Act
        var result = await handler.Handle(getOrder, _cancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<OrderDto>>();
        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetCustomerOrders_ShouldReturnOrders_WhenOrdersExist()
    {
        // Arrange
        var logger = Substitute.For<ILoggerAdapter<GetCustomerOrdersQueryHandler>>();
        
        var getOrders = new GetCustomerOrdersQuery(_customer.Id);
        var handler = new GetCustomerOrdersQueryHandler(
            _customerRepository,
            logger);
        
        // Act
        var result = await handler.Handle(getOrders, _cancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<List<OrderDto>>>();
        result.Value.Should().NotBeNull();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().HaveCount(1);
        result.Value.First().Should().Be(OrderDto.From(_order));
    }
    
    [Fact]
    public async Task GetCustomerOrders_ShouldReturnFailure_WhenCustomerDoesNotExist()
    {
        // Arrange
        var getOrders = new GetCustomerOrdersQuery(CustomerId.From(Guid.NewGuid()));
        var logger = Substitute.For<ILoggerAdapter<GetCustomerOrdersQueryHandler>>();
        var handler = new GetCustomerOrdersQueryHandler(
            _customerRepository,
            logger);
        
        // Act
        var result = await handler.Handle(getOrders, _cancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<List<OrderDto>>>();
        result.IsFailure.Should().BeTrue();
    }
    
    [Fact]
    public async Task GetCustomerOrderDetails_ShouldReturnOrderDetails_WhenOrderExists()
    {
        // Arrange
        var getOrder = new GetCustomerOrderDetailsQuery(
            _order.Id,
            _customer.Id);

        var logger = Substitute.For<ILoggerAdapter<GetCustomerOrderDetailsQueryHandler>>();
        var handler = new GetCustomerOrderDetailsQueryHandler(
            _customerRepository,
            logger);
        
        // Act
        var result = await handler.Handle(getOrder, _cancellationToken);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<OrderDetailsDto>>();
        result.Value.Should().Be(OrderDetailsDto.From(_order));
    }
}