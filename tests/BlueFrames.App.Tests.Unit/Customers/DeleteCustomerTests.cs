using BlueFrames.Application.Customers.Commands.DeleteCustomer;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.App.Tests.Unit.Customers;

public class DeleteCustomerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILoggerAdapter<DeleteCustomerCommandHandler> _logger = Substitute.For<ILoggerAdapter<DeleteCustomerCommandHandler>>();

    private readonly Customer _customer;

    public DeleteCustomerTests()
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
    public async Task DeleteCustomer_ShouldSuccess_WhenCustomerExists()
    {
        // Arrange        
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);

        var deleteCustomer = new DeleteCustomerCommand(_customer.Id);
        var deleteHandler = new DeleteCustomerCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var deleteResult = await deleteHandler.Handle(deleteCustomer, _cancellationToken);
        
        // Assert
        deleteResult.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteCustomer_ShouldReturnFailure_WhenCustomerNotFound()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var deleteCustomer = new DeleteCustomerCommand(CustomerId.From(Guid.NewGuid()));
        var deleteHandler = new DeleteCustomerCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var deleteResult = await deleteHandler.Handle(deleteCustomer, _cancellationToken);
        
        // Assert
        deleteResult.IsSuccess.Should().BeFalse();
    }
}