using BlueFrames.Domain.Customers;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class CustomerTests
{
    private readonly Customer _customer;
    private readonly Bogus.Person _person;
    public CustomerTests()
    {
        _person = new Bogus.Person(locale: "en_GB");
        _customer = new Customer(_person.FirstName, _person.LastName, _person.Phone, _person.Email);
    }

    [Fact]
    public void Create_ShouldInitialiseCustomerWithValidPersonalDetails()
    {
        //Assert
        _customer.FirstName.Should().Be(_person.FirstName);
        _customer.LastName.Should().Be(_person.LastName);
        _customer.Phone.Should().Be(_person.Phone);
        _customer.Email.Should().Be(_person.Email);
    }
    
    [Fact]
    public void Create_ShouldInitialiseCustomerWithValidId()
    {
        //Assert
        _customer.Id.Should().NotBeEmpty();
    }
}