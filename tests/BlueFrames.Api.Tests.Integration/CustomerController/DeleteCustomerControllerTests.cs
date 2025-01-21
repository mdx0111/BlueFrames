using BlueFrames.Api.Contracts.Customers.Requests;

namespace BlueFrames.Api.Tests.Integration.CustomerController;

public class DeleteCustomerControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly Faker<CustomerRequest> _customerFaker = new Faker<CustomerRequest>("en_GB")
        .RuleFor(dto => dto.FirstName, faker => faker.Person.FirstName)
        .RuleFor(dto => dto.LastName, faker => faker.Person.LastName)
        .RuleFor(dto => dto.Phone, faker => faker.Phone.PhoneNumberFormat(1).Replace(" ", string.Empty))
        .RuleFor(dto => dto.Email, faker => faker.Person.Email);

    private readonly HttpClient _userHttpClient;
    private readonly HttpClient _adminHttpClient;
    private readonly HttpClient _httpClient;

    public DeleteCustomerControllerTests(BlueFramesApiFactory factory)
    {
        _adminHttpClient = factory.CreateHttpClientWithAdminCredentials();
        _userHttpClient = factory.CreateHttpClientWithUserCredentials();
        _httpClient = factory.CreateClient();
    }
    
    [Fact]
    public async Task Delete_ShouldReturnOk_WhenCustomerExist()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;
        
        // Act
        var response = await _adminHttpClient.DeleteAsync($"/api/v1/Customer/{customerId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Act
        var response = await _adminHttpClient.DeleteAsync($"/api/v1/Customer/{Guid.NewGuid()}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var validationError = await response.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("Customer not found");
    }
    
    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;
        
        // Act
        var response = await _userHttpClient.DeleteAsync($"/api/v1/Customer/{customerId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;
        
        // Act
        var response = await _httpClient.DeleteAsync($"/api/v1/Customer/{customerId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}