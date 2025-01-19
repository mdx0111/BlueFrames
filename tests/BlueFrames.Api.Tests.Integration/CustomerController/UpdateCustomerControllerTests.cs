using BlueFrames.Api.Contracts.Customers.Requests;
using BlueFrames.Api.Contracts.Customers.Responses;

namespace BlueFrames.Api.Tests.Integration.CustomerController;

public class UpdateCustomerControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _httpClient;

    private readonly Faker<CustomerRequest> _customerFaker = new Faker<CustomerRequest>("en_GB")
        .RuleFor(dto => dto.FirstName, faker => faker.Person.FirstName)
        .RuleFor(dto => dto.LastName, faker => faker.Person.LastName)
        .RuleFor(dto => dto.Phone, faker => faker.Phone.PhoneNumberFormat(1).Replace(" ", string.Empty))
        .RuleFor(dto => dto.Email, faker => faker.Person.Email);
    
    public UpdateCustomerControllerTests(BlueFramesApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateCustomer_ShouldReturnOk_WhenCustomerIsUpdated()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createResponse = await _httpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var createResponseContent = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = createResponseContent.Result;

        var updatedCustomer = _customerFaker.Generate();

        // Act
        var updateResponse = await _httpClient.PutAsJsonAsync($"/api/v1/Customer/{customerId}", updatedCustomer);
        var getResponse = await _httpClient.GetAsync($"/api/v1/Customer/{customerId}");
        var getCustomerResponse = await getResponse.Content.ReadFromJsonAsync<Envelope<CustomerResponse>>();

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getCustomerResponse.Result.Should().BeEquivalentTo(new CustomerResponse
        {
            Id = Guid.Parse(customerId),
            FirstName = updatedCustomer.FirstName,
            LastName = updatedCustomer.LastName,
            Phone = updatedCustomer.Phone,
            Email = updatedCustomer.Email
        });
    }
    
    [Fact]
    public async Task UpdateCustomer_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createResponse = await _httpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var createResponseContent = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = createResponseContent.Result;

        var updatedCustomer = _customerFaker.Clone()
            .RuleFor(x => x.Email, "invalid-email")
            .Generate();

        // Act
        var updateResponse = await _httpClient.PutAsJsonAsync($"/api/v1/Customer/{customerId}", updatedCustomer);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationError = await updateResponse.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("is not a valid email address");
    }
}