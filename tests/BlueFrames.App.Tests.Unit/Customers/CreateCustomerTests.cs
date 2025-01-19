using BlueFrames.Application.Customers.Commands.CreateCustomer;

namespace BlueFrames.App.Tests.Unit.Customers;

public class CreateCustomerTests
{ 
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ICustomerRepository _repository = Substitute.For<ICustomerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILoggerAdapter<CreateCustomerCommandHandler> _logger = Substitute.For<ILoggerAdapter<CreateCustomerCommandHandler>>();
    private readonly Bogus.Person _person;
    private const string ValidPhoneNumber = "07563385651";

    public CreateCustomerTests()
    {
        _person = new Bogus.Person(locale: "en_GB");
    }

    [Fact]
    public async Task CreateCustomer_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var createCustomer = new CreateCustomerCommand(
            _person.FirstName,
            _person.LastName,
            ValidPhoneNumber,
            _person.Email);

        var handler = new CreateCustomerCommandHandler(_repository, _unitOfWork, _logger);
        
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
            _person.FirstName,
            _person.LastName,
            ValidPhoneNumber,
            "");
        var handler = new CreateCustomerCommandHandler(_repository, _unitOfWork, _logger);

        // Act
        var result = await handler.Handle(createCustomer, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeEmpty();
    }
}