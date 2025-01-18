using BlueFrames.Application.Customers.Commands.CreateCustomer;
using BlueFrames.Application.Customers.Commands.UpdateCustomer;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.App.Tests.Unit.Customers;

public class UpdateCustomerTests
{
    [Fact]
    public async Task UpdateCustomer_ShouldSuccess_WhenGivenValidData()
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
        
        var updateCustomer = new UpdateCustomerCommand(
            customer.Id.Value,
            "Jane",
            "Doee",
            "07512345671",
            "jan@doee.com");
        var updateHandler = new UpdateCustomerCommandHandler(repository, unitOfWork);
        
        // Act
        var updateResult = await updateHandler.Handle(updateCustomer, cancellationToken);
        
        // Assert
        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.Should().NotBeEmpty();
        updateResult.Value.Should().Be(customer.Id.Value);
        
        customer.FirstName.Value.Should().Be(updateCustomer.FirstName);
        customer.LastName.Value.Should().Be(updateCustomer.LastName);
        customer.Phone.Value.Should().Be(updateCustomer.Phone);
        customer.Email.Value.Should().Be(updateCustomer.Email);
    }
}