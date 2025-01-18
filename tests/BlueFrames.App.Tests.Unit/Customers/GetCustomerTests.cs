using BlueFrames.Application.Common.Results;
using BlueFrames.Application.Customers.Queries.Common;
using BlueFrames.Application.Customers.Queries.GetAllCustomers;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.App.Tests.Unit.Customers;

public class GetCustomerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private const string ValidPhoneNumber = "07563385651";

    [Fact]
    public async Task GetAllCustomers_ShouldReturnCustomers_WhenExists()
    {
        // Arrange
        var listOfCustomers = new List<Customer>();
        for (var index = 0; index < 10; index++)
        {
            var person = new Bogus.Person(locale: "en_GB");
            var customer = Customer.Create(
                FirstName.From(person.FirstName),
                LastName.From(person.LastName),
                PhoneNumber.From(ValidPhoneNumber),
                Email.From(person.Email));
            listOfCustomers.Add(customer);
        }

        _repository.GetAllAsync(10, 0, _cancellationToken).Returns(listOfCustomers);
        
        var query = new GetAllCustomersQuery(10, 0);
        var handler = new GetAllCustomersQueryHandler(_repository);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<List<CustomerDto>>>();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<List<CustomerDto>>();
        result.Value.Count.Should().Be(listOfCustomers.Count);
    }
}