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
    [ClassData(typeof(InvalidFirstNames))]
    public void Create_ShouldThrowException_WhenFirstNameIsInvalid(string firstName)
    {
        // Act
        Action createCustomer = () => _ = Customer.Create(FirstName.From(firstName), _customer.LastName, _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }

    [Theory]
    [ClassData(typeof(InvalidLastNames))]
    public void Create_ShouldThrowException_WhenLastNameIsInvalid(string lastName)
    {
        // Act
        Action createCustomer = () => _ = Customer.Create(_customer.FirstName, LastName.From(lastName), _customer.Phone, _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Theory]
    [ClassData(typeof(InvalidPhoneNumbers))]
    public void Create_ShouldThrowException_WhenPhoneNumberIsInvalid(string phoneNumber)
    {
        // Act
        Action createCustomer = () => _ = Customer.Create(_customer.FirstName, _customer.LastName, PhoneNumber.From(phoneNumber), _customer.Email);

        //Assert
        createCustomer.Should().Throw<ValidationException>();
    }
    
    [Theory]
    [ClassData(typeof(InvalidEmailAddresses))]
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
    [ClassData(typeof(InvalidFirstNames))]
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
    [ClassData(typeof(InvalidLastNames))]
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
    [ClassData(typeof(InvalidPhoneNumbers))]
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
    [ClassData(typeof(InvalidEmailAddresses))]
    public void ChangeEmail_ShouldThrowException_WhenEmailIsInvalid(string email)
    {
        // Act
        Action changeEmail = () => _customer.ChangeEmail(Email.From(email));

        //Assert
        changeEmail.Should().Throw<ValidationException>();
    }
}

public class InvalidFirstNames : TheoryData<string>
{
    public InvalidFirstNames()
    {
        Add("A");
        Add("$");
        Add("Doe123");
        Add("@Doe");
        Add(string.Empty);
        Add(null);
    }
}

public class InvalidLastNames : TheoryData<string>
{
    public InvalidLastNames()
    {
        Add("A");
        Add("$");
        Add("Doe123");
        Add("@Doe");
        Add(string.Empty);
        Add(null);
    }
}

public class InvalidPhoneNumbers : TheoryData<string>
{
    public InvalidPhoneNumbers()
    {
        Add("075633856515");
        Add("0A7563385651");
        Add("07563-385651");
        Add("0756338565");
        Add("+44956338565");
        Add(string.Empty);
        Add(null);
    }
}

public class InvalidEmailAddresses : TheoryData<string>
{
    public InvalidEmailAddresses()
    {
        Add("name");
        Add("name@");
        Add("name@domain");
        Add("@domain");
        Add("@domain.com");
        Add("domain.com");
        Add(string.Empty);
        Add(null);
    }
}