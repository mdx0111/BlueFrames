using BlueFrames.Application.Common.Results;
using BlueFrames.Application.Customers.Queries.Common;
using BlueFrames.Application.Customers.Queries.GetAllCustomers;
using BlueFrames.Application.Customers.Queries.GetCustomerById;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;
using NSubstitute.ReturnsExtensions;

namespace BlueFrames.App.Tests.Unit.Customers;

public class GetCustomerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly List<Customer> _listOfCustomers;

    public GetCustomerTests()
    {
        var faker = new Bogus.Faker("en_GB");

        _listOfCustomers = [];
        
        for (var index = 0; index < 10; index++)
        {
            var person = new Bogus.Person(locale: "en_GB");
            var customer = Customer.Create(
                FirstName.From(person.FirstName),
                LastName.From(person.LastName),
                PhoneNumber.From(faker.Phone.PhoneNumberFormat(1)),
                Email.From(person.Email));
            _listOfCustomers.Add(customer);
        }
    }

    [Fact]
    public async Task GetAllCustomers_ShouldReturnCustomers_WhenExists()
    {
        // Arrange
        _repository.GetAllAsync(10, 0, _cancellationToken).Returns(_listOfCustomers);
        
        var query = new GetAllCustomersQuery(10, 0);
        var logger = Substitute.For<ILoggerAdapter<GetAllCustomersQueryHandler>>();
        var handler = new GetAllCustomersQueryHandler(_repository, logger);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<List<CustomerDto>>>();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<List<CustomerDto>>();
        result.Value.Count.Should().Be(_listOfCustomers.Count);
    }
    
    [Fact]
    public async Task GetAllCustomers_ShouldReturnFailure_WhenPageIsEmpty()
    {
        // Arrange
        _repository.GetAllAsync(10, 10, _cancellationToken).Returns([]);
        
        var query = new GetAllCustomersQuery(10, 0);
        var logger = Substitute.For<ILoggerAdapter<GetAllCustomersQueryHandler>>();
        var handler = new GetAllCustomersQueryHandler(_repository, logger);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<List<CustomerDto>>>();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
    }
    
    [Fact]
    public async Task GetCustomer_ShouldReturnCustomer_WhenFound()
    {
        // Arrange
        var firstCustomer = _listOfCustomers.First();
        _repository.GetByIdAsync(firstCustomer.Id.Value, _cancellationToken).Returns(firstCustomer);
        
        var query = new GetCustomerByIdQuery(firstCustomer.Id);
        var logger = Substitute.For<ILoggerAdapter<GetCustomerByIdQueryHandler>>();
        var handler = new GetCustomerByIdQueryHandler(_repository, logger);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<CustomerDto>>();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(CustomerDto.From(firstCustomer));
    }
    
    [Fact]
    public async Task GetCustomer_ShouldReturnFailure_WhenNotFound()
    {
        // Arrange
        var firstCustomer = _listOfCustomers.First();
        _repository.GetByIdAsync(firstCustomer.Id.Value, _cancellationToken).ReturnsNull();
        
        var query = new GetCustomerByIdQuery(firstCustomer.Id);
        var logger = Substitute.For<ILoggerAdapter<GetCustomerByIdQueryHandler>>();
        var handler = new GetCustomerByIdQueryHandler(_repository, logger);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<CustomerDto>>();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}