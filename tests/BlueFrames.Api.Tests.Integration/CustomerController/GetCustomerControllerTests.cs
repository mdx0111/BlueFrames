using BlueFrames.Api.Contracts.Customers.Requests;
using BlueFrames.Api.Contracts.Customers.Responses;

namespace BlueFrames.Api.Tests.Integration.CustomerController;

[TestCaseOrderer("BlueFrames.Api.Tests.Integration.Helpers.PriorityOrderer", "BlueFrames.Api.Tests.Integration")]
public class GetCustomerControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly Faker<CustomerRequest> _customerFaker = new Faker<CustomerRequest>("en_GB")
        .RuleFor(dto => dto.FirstName, faker => faker.Person.FirstName)
        .RuleFor(dto => dto.LastName, faker => faker.Person.LastName)
        .RuleFor(dto => dto.Phone, faker => faker.Phone.PhoneNumberFormat(1).Replace(" ", string.Empty))
        .RuleFor(dto => dto.Email, faker => faker.Person.Email);

    private readonly HttpClient _adminHttpClient;
    private readonly HttpClient _userHttpClient;

    public GetCustomerControllerTests(BlueFramesApiFactory factory)
    {
        _adminHttpClient = factory.CreateHttpClientWithAdminCredentials();
        _userHttpClient = factory.CreateHttpClientWithUserCredentials();
    }

    [Fact, TestPriority(4)]
    public async Task Get_ShouldReturnCustomer_WhenCustomerExist()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;

        // Act
        var response = await _adminHttpClient.GetAsync($"/api/v1/Customer/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getCustomerResponse = await response.Content.ReadFromJsonAsync<Envelope<CustomerResponse>>();
        getCustomerResponse.Result.Should().BeEquivalentTo(new CustomerResponse
        {
            Id = Guid.Parse(customerId),
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.Phone,
            Email = customer.Email
        });
    }
    
    [Fact, TestPriority(3)]
    public async Task Get_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Act
        var response = await _adminHttpClient.GetAsync($"/api/v1/Customer/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var validationError = await response.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("Customer not found");
    }
    
    [Fact, TestPriority(1)]
    public async Task GetAll_ShouldNotReturnCustomers_WhenCustomerDoesNotExist()
    {
        // Act
        var response = await _adminHttpClient.GetAsync("/api/v1/Customer?offset=0&limit=10");
    
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customersResponse = await response.Content.ReadFromJsonAsync<Envelope<List<CustomerResponse>>>();
        customersResponse.Result.Should().BeEmpty();
    }
    
    [Fact, TestPriority(2)]
    public async Task GetAll_ShouldReturnCustomers_WhenCustomersExist()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;

        // Act
        var response = await _adminHttpClient.GetAsync("/api/v1/Customer?offset=0&limit=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getCustomerResponse = await response.Content.ReadFromJsonAsync<Envelope<List<CustomerResponse>>>();
        getCustomerResponse.Result.Should().NotBeEmpty();
        getCustomerResponse.Result.Should().ContainEquivalentOf(new CustomerResponse
        {
            Id = Guid.Parse(customerId),
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.Phone,
            Email = customer.Email
        });
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        // Act
        var response = await _userHttpClient.GetAsync("/api/v1/Customer?offset=0&limit=10");
    
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}