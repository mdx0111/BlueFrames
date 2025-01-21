using BlueFrames.Api.Contracts.Customers.Requests;

namespace BlueFrames.Api.Tests.Integration.CustomerController;

public class CreateCustomerControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _httpClient;

    private readonly Faker<CustomerRequest> _customerFaker = new Faker<CustomerRequest>("en_GB")
        .RuleFor(dto => dto.FirstName, faker => faker.Person.FirstName)
        .RuleFor(dto => dto.LastName, faker => faker.Person.LastName)
        .RuleFor(dto => dto.Phone, faker => faker.Phone.PhoneNumberFormat(1))
        .RuleFor(dto => dto.Email, faker => faker.Person.Email);
    
    public CreateCustomerControllerTests(BlueFramesApiFactory factory)
    {
        _httpClient = factory.CreateHttpClientWithAdminCredentials();
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenCustomerIsCreated()
    {
        // Arrange
        var customer = _customerFaker.Generate();

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/Customer", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createResponse = await response.Content.ReadFromJsonAsync<Envelope>();
        createResponse.Should().NotBeNull();
        createResponse.Result.Should().NotBeEmpty();
        createResponse.Result.Should().NotBe(Guid.Empty.ToString());
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenCustomerIsInvalid()
    {
        // Arrange
        var customer = _customerFaker.Clone()
            .RuleFor(dto => dto.FirstName, string.Empty)
            .Generate();

        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/Customer", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationError = await response.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("is not a valid first name");
    }
}