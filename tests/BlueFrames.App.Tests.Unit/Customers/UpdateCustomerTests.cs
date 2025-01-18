using BlueFrames.Application.Customers.Commands.UpdateCustomer;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.App.Tests.Unit.Customers;

public class UpdateCustomerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly Customer _customer = Customer.Create(
        FirstName.From("John"),
        LastName.From("Doe"),
        PhoneNumber.From("07563385651"),
        Email.From("john@doe.com"));
    
    public UpdateCustomerTests()
    {
        _repository.GetByIdAsync(_customer.Id.Value, _cancellationToken).Returns(_customer);
    }
    
    [Fact]
    public async Task UpdateCustomer_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var updateCustomer = new UpdateCustomerCommand(
            _customer.Id.Value,
            "Jane",
            "Doee",
            "07512345671",
            "jan@doee.com");
        var updateHandler = new UpdateCustomerCommandHandler(_repository, _unitOfWork);
        
        // Act
        var updateResult = await updateHandler.Handle(updateCustomer, _cancellationToken);
        
        // Assert
        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.Should().NotBeEmpty();
        updateResult.Value.Should().Be(_customer.Id.Value);
        
        _customer.FirstName.Value.Should().Be(updateCustomer.FirstName);
        _customer.LastName.Value.Should().Be(updateCustomer.LastName);
        _customer.Phone.Value.Should().Be(updateCustomer.Phone);
        _customer.Email.Value.Should().Be(updateCustomer.Email);
    }
    
    [Fact]
    public async Task UpdateCustomer_ShouldFail_WhenGivenInvalidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var updateCustomer = new UpdateCustomerCommand(
            _customer.Id.Value,
            "Jane",
            "Doee",
            "07512345671",
            "");
        var updateHandler = new UpdateCustomerCommandHandler(_repository, _unitOfWork);
        
        // Act
        var updateResult = await updateHandler.Handle(updateCustomer, _cancellationToken);
        
        // Assert
        updateResult.IsSuccess.Should().BeFalse();
        updateResult.Value.Should().BeEmpty();
    }
    
    [Fact]
    public async Task UpdateCustomer_ShouldFail_WhenCustomerNotFound()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var updateCustomer = new UpdateCustomerCommand(
            Guid.NewGuid(),
            "Jane",
            "Doee",
            "07512345671",
            "jan@doee.com");
        var updateHandler = new UpdateCustomerCommandHandler(_repository, _unitOfWork);
        
        // Act
        var updateResult = await updateHandler.Handle(updateCustomer, _cancellationToken);
        
        // Assert
        updateResult.IsSuccess.Should().BeFalse();
        updateResult.Value.Should().BeEmpty();
    }
}