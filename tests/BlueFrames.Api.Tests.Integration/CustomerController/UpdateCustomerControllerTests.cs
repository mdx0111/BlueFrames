using BlueFrames.Api.Models.Customers;

namespace BlueFrames.Api.Tests.Integration.CustomerController;

public class UpdateCustomerControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _httpClient;

    private readonly Faker<CustomerRequest> _customerFaker = new Faker<CustomerRequest>("en_GB")
        .RuleFor(dto => dto.FirstName, faker => faker.Person.FirstName)
        .RuleFor(dto => dto.LastName, faker => faker.Person.LastName)
        .RuleFor(dto => dto.Phone, faker => faker.Phone.PhoneNumberFormat(1))
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
        var response = await _httpClient.PutAsJsonAsync($"/api/v1/Customer/{customerId}", updatedCustomer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}