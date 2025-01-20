using BlueFrames.Api.Contracts.Customers.Requests;
using BlueFrames.Api.Contracts.Orders.Requests;
using BlueFrames.Api.Contracts.Products.Requests;

namespace BlueFrames.Api.Tests.Integration.OrderController;

public class PlaceOrderControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _httpClient;
    
    private const int ProductSKUCharacterCount = 5;
    private readonly Faker<ProductRequest> _productFaker = new Faker<ProductRequest> ("en_GB")
        .RuleFor(dto => dto.Name, f => f.Commerce.ProductName())
        .RuleFor(dto => dto.Description, f => f.Commerce.ProductDescription())
        .RuleFor(dto => dto.SKU, f => f.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper());
    
    private readonly Faker<CustomerRequest> _customerFaker = new Faker<CustomerRequest>("en_GB")
        .RuleFor(dto => dto.FirstName, faker => faker.Person.FirstName)
        .RuleFor(dto => dto.LastName, faker => faker.Person.LastName)
        .RuleFor(dto => dto.Phone, faker => faker.Phone.PhoneNumberFormat(1))
        .RuleFor(dto => dto.Email, faker => faker.Person.Email);

    public PlaceOrderControllerTests(BlueFramesApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task PlaceOrder_ShouldReturnCreated_WhenOrderIsCreated()
    {
        // Arrange
        var product = _productFaker.Generate();
        var customer = _customerFaker.Generate();
        
        var createProductResponse = await _httpClient.PostAsJsonAsync("/api/v1/Product", product);
        var createProductResponseContent = await createProductResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = createProductResponseContent.Result;
        
        var createCustomerResponse = await _httpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var createCustomerResponseContent = await createCustomerResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = createCustomerResponseContent.Result;
        
        var placeOrderRequest = new PlaceOrderRequest
        {
            CustomerId = Guid.Parse(customerId),
            ProductId = Guid.Parse(productId)
        };
        
        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/Order", placeOrderRequest);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createResponse = await response.Content.ReadFromJsonAsync<Envelope>();
        createResponse.Should().NotBeNull();
        createResponse.Result.Should().NotBeEmpty();
        createResponse.Result.Should().NotBe(Guid.Empty.ToString());
    }
    
    [Fact]
    public async Task PlaceOrder_ShouldReturnBadRequest_WhenProductIdIsInvalid()
    {
        // Arrange
        var customer = _customerFaker.Generate();
       
        var createCustomerResponse = await _httpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var createCustomerResponseContent = await createCustomerResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = createCustomerResponseContent.Result;
        
        var placeOrderRequest = new PlaceOrderRequest
        {
            CustomerId = Guid.Parse(customerId),
            ProductId = Guid.NewGuid()
        };
        
        // Act
        var response = await _httpClient.PostAsJsonAsync("/api/v1/Order", placeOrderRequest);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationError = await response.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("Product not found");
    }
}