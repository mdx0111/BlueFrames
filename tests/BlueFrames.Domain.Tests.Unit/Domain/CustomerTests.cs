using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;
using FluentValidation;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class CustomerTests
{
    private readonly Customer _customer;
    private readonly Bogus.Person _person;
    private const string ValidPhoneNumber = "07563385651";
    
    public CustomerTests()
    {
        _person = new Bogus.Person(locale: "en_GB");
        _customer = new Customer(
            FirstName.From(_person.FirstName),
            LastName.From(_person.LastName),
            PhoneNumber.From(ValidPhoneNumber),
            Email.From(_person.Email));
    }

    [Fact]
    public void Create_ShouldInitialiseCustomerWithValidPersonalDetails()
    {
        //Assert
        _customer.FirstName.Value.Should().Be(_person.FirstName);
        _customer.LastName.Value.Should().Be(_person.LastName);
        _customer.Phone.Value.Should().Be(ValidPhoneNumber);
        _customer.Email.Value.Should().Be(_person.Email);
    }
    
    [Fact]
    public void Create_ShouldInitialiseCustomerWithValidId()
    {
        //Assert
        _customer.Id.Value.Should().NotBeEmpty();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenFirstNameIsNull()
    {
        // Act
        Action createCustomer = () => _ = new Customer(null, _customer.LastName, _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenFirstNameIsEmpty()
    {
        // Act
        Action createCustomer = () => _ = new Customer(FirstName.From(string.Empty), _customer.LastName, _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenFirstNameIsInvalid()
    {
        // Act
        Action createCustomer = () => _ = new Customer(FirstName.From("$"), _customer.LastName, _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenLastNameIsNull()
    {
        // Act
        Action createCustomer = () => _ = new Customer(_customer.FirstName, null, _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenLastNameIsEmpty()
    {
        // Act
        Action createCustomer = () => _ = new Customer(_customer.FirstName, LastName.From(string.Empty), _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Create_ShouldThrowException_WhenLastNameIsInvalid()
    {
        // Act
        Action createCustomer = () => _ = new Customer(_customer.FirstName, LastName.From("#"), _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenPhoneNumberIsNull()
    {
        // Act
        Action createCustomer = () => _ = new Customer(_customer.FirstName, _customer.LastName, null, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenPhoneNumberIsEmpty()
    {
        // Act
        Action createCustomer = () => _ = new Customer(_customer.FirstName, _customer.LastName, PhoneNumber.From(string.Empty), _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }

    [Fact] public void Create_ShouldThrowException_WhenPhoneNumberIsInvalid()
    {
        // Act
        Action createCustomer = () => _ = new Customer(_customer.FirstName, _customer.LastName, PhoneNumber.From("075633856515"), _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenEmailIsNull()
    {
        // Act
        Action createCustomer = () => _ = new Customer(_customer.FirstName, _customer.LastName, _customer.Phone, null);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenEmailIsEmpty()
    {
        // Act
        Action createCustomer = () => _ = new Customer(_customer.FirstName, _customer.LastName, _customer.Phone, Email.From(string.Empty));

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }

    [Fact] public void Create_ShouldThrowException_WhenEmailIsInvalid()
    {
        // Act
        Action createCustomer = () => _ = new Customer(_customer.FirstName, _customer.LastName, _customer.Phone, Email.From("test.com"));

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
}