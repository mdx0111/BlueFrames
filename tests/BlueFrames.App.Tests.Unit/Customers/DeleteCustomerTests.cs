using BlueFrames.Application.Customers.Commands.DeleteCustomer;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.App.Tests.Unit.Customers;

public class DeleteCustomerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly Customer _customer = Customer.Create(
        FirstName.From("John"),
        LastName.From("Doe"),
        PhoneNumber.From("07563385651"),
        Email.From("john@doe.com"));

    public DeleteCustomerTests()
    {
        _repository.GetByIdAsync(_customer.Id.Value, _cancellationToken).Returns(_customer);
    }
    
    [Fact]
    public async Task DeleteCustomer_ShouldSuccess_WhenUserExists()
    {
        // Arrange        
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);

        var deleteCustomer = new DeleteCustomerCommand(_customer.Id.Value);
        var deleteHandler = new DeleteCustomerCommandHandler(_repository, _unitOfWork);
        
        // Act
        var deleteResult = await deleteHandler.Handle(deleteCustomer, _cancellationToken);
        
        // Assert
        deleteResult.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteCustomer_ShouldFail_WhenCustomerNotFound()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var deleteCustomer = new DeleteCustomerCommand(Guid.NewGuid());
        var deleteHandler = new DeleteCustomerCommandHandler(_repository, _unitOfWork);
        
        // Act
        var updateResult = await deleteHandler.Handle(deleteCustomer, _cancellationToken);
        
        // Assert
        updateResult.IsSuccess.Should().BeFalse();
    }
}