using BlueFrames.Api.Contracts;
using BlueFrames.Api.Contracts.Customers.Requests;
using BlueFrames.Api.Contracts.Customers.Responses;

namespace BlueFrames.Api.Tests.Integration.CustomerController;

public class GetCustomerControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _httpClient;

    private readonly Faker<CustomerRequest> _customerFaker = new Faker<CustomerRequest>("en_GB")
        .RuleFor(dto => dto.FirstName, faker => faker.Person.FirstName)
        .RuleFor(dto => dto.LastName, faker => faker.Person.LastName)
        .RuleFor(dto => dto.Phone, faker => faker.Phone.PhoneNumberFormat(1).Replace(" ", string.Empty))
        .RuleFor(dto => dto.Email, faker => faker.Person.Email);
    
    public GetCustomerControllerTests(BlueFramesApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ShouldReturnCustomer_WhenCustomerExist()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createResponse = await _httpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/Customer/{customerId}");

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
    
    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenCustomerDoesNotExist()
    {
        // Act
        var response = await _httpClient.GetAsync($"/api/v1/Customer/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var validationError = await response.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("Customer not found");
    }
}