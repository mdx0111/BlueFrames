using BlueFrames.Application.Customers.Commands.DeleteCustomer;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.App.Tests.Unit.Customers;

public class DeleteCustomerTests
{
    [Fact]
    public async Task DeleteCustomer_ShouldSuccess_WhenUserExists()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        
        var customer = Customer.Create(
            FirstName.From("John"),
            LastName.From("Doe"),
            PhoneNumber.From("07563385651"),
            Email.From("john@doe.com"));
        
        var repository = Substitute.For<ICustomerRepository>();
        repository.GetByIdAsync(customer.Id.Value, cancellationToken).Returns(customer);
        
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.SaveChangesAsync(cancellationToken).Returns(1);

        var deleteCustomer = new DeleteCustomerCommand(customer.Id.Value);
        var deleteHandler = new DeleteCustomerCommandHandler(repository, unitOfWork);
        
        // Act
        var deleteResult = await deleteHandler.Handle(deleteCustomer, cancellationToken);
        
        // Assert
        deleteResult.IsSuccess.Should().BeTrue();
    }
}