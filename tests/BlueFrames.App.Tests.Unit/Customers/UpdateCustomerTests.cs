using BlueFrames.Application.Customers.Commands.UpdateCustomer;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.App.Tests.Unit.Customers;

public class UpdateCustomerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILoggerAdapter<UpdateCustomerCommandHandler> _logger = Substitute.For<ILoggerAdapter<UpdateCustomerCommandHandler>>();

    private readonly Customer _customer;
    
    public UpdateCustomerTests()
    {
        var faker = new Bogus.Faker("en_GB");
        var person = new Bogus.Person(locale: "en_GB");
        _customer = Customer.Create(
            FirstName.From(person.FirstName),
            LastName.From(person.LastName),
            PhoneNumber.From(faker.Phone.PhoneNumberFormat(1)),
            Email.From(person.Email));

        _repository.GetByIdAsync(_customer.Id.Value, _cancellationToken).Returns(_customer);
    }
    
    [Fact]
    public async Task UpdateCustomer_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var person = new Bogus.Person(locale: "en_GB");
        var updateCustomer = new UpdateCustomerCommand(
            _customer.Id,
            FirstName.From(person.FirstName),
            LastName.From(person.LastName),
            PhoneNumber.From("07512345671"),
            Email.From(person.Email));
        var updateHandler = new UpdateCustomerCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var updateResult = await updateHandler.Handle(updateCustomer, _cancellationToken);
        
        // Assert
        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.Should().NotBeEmpty();
        updateResult.Value.Should().Be(_customer.Id.Value);
        
        _customer.FirstName.Should().Be(updateCustomer.FirstName);
        _customer.LastName.Should().Be(updateCustomer.LastName);
        _customer.Phone.Should().Be(updateCustomer.Phone);
        _customer.Email.Should().Be(updateCustomer.Email);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldReturnFailure_WhenCustomerNotFound()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var person = new Bogus.Person(locale: "en_GB");
        var updateCustomer = new UpdateCustomerCommand(
            CustomerId.From(Guid.NewGuid()),
            FirstName.From(person.FirstName),
            LastName.From(person.LastName),
            PhoneNumber.From("07512345671"),
            Email.From(person.Email));
        var updateHandler = new UpdateCustomerCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var updateResult = await updateHandler.Handle(updateCustomer, _cancellationToken);
        
        // Assert
        updateResult.IsSuccess.Should().BeFalse();
        updateResult.Value.Should().BeEmpty();
    }
}