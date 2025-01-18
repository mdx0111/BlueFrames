using BlueFrames.Application.Customers.Commands.CreateCustomer;

namespace BlueFrames.App.Tests.Unit.Customers;

public class CreateCustomerTests
{ 
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    
    [Fact]
    public async Task CreateCustomer_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var createCustomer = new CreateCustomerCommand(
            "John",
            "Doe",
            "07512345671",
            "john@doe.com");
        var handler = new CreateCustomerCommandHandler(_repository, _unitOfWork);
        
        // Act
        var result = await handler.Handle(createCustomer, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateCustomer_ShouldFail_WhenGivenInvalidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(0);

        var createCustomer = new CreateCustomerCommand(
            "John",
            "Doe",
            "07512345671",
            "");
        var handler = new CreateCustomerCommandHandler(_repository, _unitOfWork);

        // Act
        var result = await handler.Handle(createCustomer, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}