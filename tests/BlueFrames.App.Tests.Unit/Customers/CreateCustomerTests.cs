using BlueFrames.Application.Customers.Commands;

namespace BlueFrames.App.Tests.Unit.Customers;

public class CreateCustomerTests
{
    [Fact]
    public async Task CreateCustomer_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var repository = Substitute.For<ICustomerRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.SaveChangesAsync(cancellationToken).Returns(1);
        
        var createCustomer = new CreateCustomerCommand(
            "John",
            "Doe",
            "07512345671",
            "john@doe.com");
        var handler = new CreateCustomerCommandHandler(repository, unitOfWork);
        
        // Act
        var result = await handler.Handle(createCustomer, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}