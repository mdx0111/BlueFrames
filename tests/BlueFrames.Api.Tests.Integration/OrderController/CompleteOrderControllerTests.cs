using BlueFrames.Api.Contracts.Customers.Requests;
using BlueFrames.Api.Contracts.Orders.Requests;
using BlueFrames.Api.Contracts.Products.Requests;

namespace BlueFrames.Api.Tests.Integration.OrderController;

public class CompleteOrderControllerTests : IClassFixture<BlueFramesApiFactory>
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
    
    public CompleteOrderControllerTests(BlueFramesApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteOrder_ShouldReturnOk_WhenOrderCompleted()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createProductResponse = await _httpClient.PostAsJsonAsync("/api/v1/Product", product);
        var createProductResponseContent = await createProductResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = createProductResponseContent.Result;
        
        var customer = _customerFaker.Generate();
        var createCustomerResponse = await _httpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var createCustomerResponseContent = await createCustomerResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = createCustomerResponseContent.Result;
        
        var placeOrderRequest = new PlaceOrderRequest
        {
            CustomerId = Guid.Parse(customerId),
            ProductId = Guid.Parse(productId)
        };
        
        var placeOrderResponse = await _httpClient.PostAsJsonAsync("/api/v1/Order", placeOrderRequest);
        var placeOrderResponseContent = await placeOrderResponse.Content.ReadFromJsonAsync<Envelope>();
        var orderId = placeOrderResponseContent.Result;

        // Act
        var completeOrderRequest = new CompleteOrderRequest
        {
            OrderId = Guid.Parse(orderId),
            CustomerId = Guid.Parse(customerId)
        };
        
        var completeOrderResponse = await _httpClient.PutAsJsonAsync("/api/v1/Order/Complete", completeOrderRequest);
        var completeOrderResponseContent = await completeOrderResponse.Content.ReadFromJsonAsync<Envelope>();
        
        // Assert
        completeOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        completeOrderResponseContent.Result.Should().Be(orderId);
    }
}