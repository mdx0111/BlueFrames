using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class CustomerTests
{
    private readonly Customer _customer;
    private readonly Bogus.Person _person;
    private const string ValidPhoneNumber = "07563385651";
    
    public CustomerTests()
    {
        _person = new Bogus.Person(locale: "en_GB");
        _customer = Customer.Create(
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
    
    [Theory]
    [MemberData(nameof(InvalidFirstNames))]
    public void Create_ShouldThrowException_WhenFirstNameIsInvalid(string firstName)
    {
        // Act
        Action createCustomer = () => _ = Customer.Create(FirstName.From(firstName), _customer.LastName, _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }

    [Theory]
    [MemberData(nameof(InvalidLastNames))]
    public void Create_ShouldThrowException_WhenLastNameIsInvalid(string lastName)
    {
        // Act
        Action createCustomer = () => _ = Customer.Create(_customer.FirstName, LastName.From(lastName), _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Theory]
    [MemberData(nameof(InvalidPhoneNumbers))]
    public void Create_ShouldThrowException_WhenPhoneNumberIsInvalid(string phoneNumber)
    {
        // Act
        Action createCustomer = () => _ = Customer.Create(_customer.FirstName, _customer.LastName, PhoneNumber.From(phoneNumber), _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Theory]
    [MemberData(nameof(InvalidEmailAddresses))]
    public void Create_ShouldThrowException_WhenEmailIsInvalid(string email)
    {
        // Act
        Action createCustomer = () => _ = Customer.Create(_customer.FirstName, _customer.LastName, _customer.Phone, Email.From(email));

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void ChangeFirstName_ShouldUpdateFirstName()
    {
        // Arrange
        var newFirstName = FirstName.From("John");

        // Act
        _customer.ChangeFirstName(newFirstName);

        //Assert
        _customer.FirstName.Value.Should().Be(newFirstName.Value);
    }
    
    [Theory]
    [MemberData(nameof(InvalidFirstNames))]
    public void ChangeFirstName_ShouldThrowException_WhenFirstNameIsInvalid(string firstName)
    {
        // Act
        Action changeFirstName = () => _customer.ChangeFirstName(FirstName.From(firstName));

        //Assert
        changeFirstName.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void ChangeLastName_ShouldUpdateLastName()
    {
        // Arrange
        var newLastName = LastName.From("Doe");

        // Act
        _customer.ChangeLastName(newLastName);

        //Assert
        _customer.LastName.Value.Should().Be(newLastName.Value);
    }
    
    [Theory]
    [MemberData(nameof(InvalidLastNames))]
    public void ChangeLastName_ShouldThrowException_WhenLastNameIsInvalid(string lastName)
    {
        // Act
        Action changeLastName = () => _customer.ChangeLastName(LastName.From(lastName));

        //Assert
        changeLastName.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void ChangePhone_ShouldUpdatePhone()
    {
        // Arrange
        var newPhone = PhoneNumber.From(ValidPhoneNumber);

        // Act
        _customer.ChangePhone(newPhone);

        //Assert
        _customer.Phone.Value.Should().Be(newPhone.Value);
    }
    
    [Theory]
    [MemberData(nameof(InvalidPhoneNumbers))]
    public void ChangePhone_ShouldThrowException_WhenPhoneNumberIsInvalid(string phoneNumber)
    {
        // Act
        Action changePhone = () => _customer.ChangePhone(PhoneNumber.From(phoneNumber));

        //Assert
        changePhone.Should().Throw<ValidationException>();
    }

    [Fact]
    public void ChangeEmail_ShouldUpdateEmail()
    {
        // Arrange
        Bogus.Faker faker = new();
        var newEmail = Email.From(faker.Person.Email);

        // Act
        _customer.ChangeEmail(newEmail);

        //Assert
        _customer.Email.Value.Should().Be(newEmail.Value);
    }
    
    [Theory]
    [MemberData(nameof(InvalidEmailAddresses))]
    public void ChangeEmail_ShouldThrowException_WhenEmailIsInvalid(string email)
    {
        // Act
        Action changeEmail = () => _customer.ChangeEmail(Email.From(email));

        //Assert
        changeEmail.Should().Throw<ValidationException>();
    }
 
    public static TheoryData<string> InvalidFirstNames =>
    [
        "A",
        "$",
        "John123",
        "John@Doe",
        string.Empty,
        null
    ];
    
    public static TheoryData<string> InvalidLastNames =>
    [
        "A",
        "$",
        "Doe123",
        "@Doe",
        string.Empty,
        null
    ];
    
    public static TheoryData<string> InvalidPhoneNumbers =>
    [
        "075633856515",
        "0A7563385651",
        "07563-385651",
        "0756338565",
        "+44956338565",
        string.Empty,
        null
    ];
    
    public static TheoryData<string> InvalidEmailAddresses =>
    [
        "name",
        "name@",
        "name@domain",
        "@domain",
        "@domain.com",
        "domain.com",
        string.Empty,
        null
    ];
}