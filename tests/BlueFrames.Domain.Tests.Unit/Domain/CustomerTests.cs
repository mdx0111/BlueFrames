using BlueFrames.Domain.Customers;
using FluentAssertions;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class CustomerTests
{
    [Fact]
    public void Create_ShouldInitialiseCustomerWithValidData()
    {
        // Arrange
        var person = new Bogus.Person(locale: "en_GB");
        var firstName = person.FirstName;
        var lastName = person.LastName;
        var phone = person.Phone;
        var email = person.Email;
        
        // Act
        var customer = new Customer(firstName, lastName, phone, email);

        //Assert
        customer.FirstName.Should().Be(firstName);
        customer.LastName.Should().Be(lastName);
        customer.Phone.Should().Be(phone);
        customer.Email.Should().Be(email);
    }
}