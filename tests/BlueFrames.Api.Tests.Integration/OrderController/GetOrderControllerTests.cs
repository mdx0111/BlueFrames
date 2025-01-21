using BlueFrames.Api.Contracts.Customers.Requests;
using BlueFrames.Api.Contracts.Orders.Requests;
using BlueFrames.Api.Contracts.Orders.Responses;
using BlueFrames.Api.Contracts.Products.Requests;
using BlueFrames.Domain.Orders.Common;

namespace BlueFrames.Api.Tests.Integration.OrderController;

public class GetOrderControllerTests : IClassFixture<BlueFramesApiFactory>
{
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

    private readonly HttpClient _adminHttpClient;
    private readonly HttpClient _httpClient;

    public GetOrderControllerTests(BlueFramesApiFactory factory)
    {
        _adminHttpClient = factory.CreateHttpClientWithAdminCredentials();
        _httpClient = factory.CreateClient();
    }
    
    [Fact]
    public async Task Get_ShouldReturnOrder_WhenOrderExist()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createProductResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createProductResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = productResponse.Result;
        
        var customer = _customerFaker.Generate();
        var createCustomerResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createCustomerResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;

        var orderRequest = new PlaceOrderRequest
        {
            CustomerId = Guid.Parse(customerId),
            ProductId = Guid.Parse(productId)
        };
        var placeOrderResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Order", orderRequest);
        var orderResponse = await placeOrderResponse.Content.ReadFromJsonAsync<Envelope>();
        var orderId = orderResponse.Result;
        
        // Act
        var getOrderResponse = await _adminHttpClient.GetAsync($"/api/v1/Order/{customerId}/{orderId}");
        
        // Assert
        getOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getOrderResult = await getOrderResponse.Content.ReadFromJsonAsync<Envelope<OrderResponse>>();
        getOrderResult.Result.CustomerId.Should().Be(Guid.Parse(customerId));
        getOrderResult.Result.ProductId.Should().Be(Guid.Parse(productId));
        getOrderResult.Result.Id.Should().Be(Guid.Parse(orderId));
        getOrderResult.Result.Status.Should().Be(Status.Pending.Name);
    }
    
    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenOrderNotExist()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createCustomerResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createCustomerResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;
        
        // Act
        var getOrderResponse = await _adminHttpClient.GetAsync($"/api/v1/Order/{customerId}/{Guid.NewGuid()}");
        
        // Assert
        getOrderResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var getOrderResult = await getOrderResponse.Content.ReadFromJsonAsync<Envelope>();
        getOrderResult.Errors["error"][0].Should().Contain("Order not found");
    }
    
    [Fact]
    public async Task Get_ShouldReturnBadRequest_WhenCustomerIdIsInvalid()
    {
        // Act
        var getOrderResponse = await _adminHttpClient.GetAsync($"/api/v1/Order/{Guid.NewGuid()}/{Guid.NewGuid()}");
        
        // Assert
        getOrderResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var getOrderResult = await getOrderResponse.Content.ReadFromJsonAsync<Envelope>();
        getOrderResult.Errors["error"][0].Should().Contain("Customer not found");
    }
    
    [Fact]
    public async Task GetOrderDetails_ShouldReturnOk_WhenOrderDetailsExist()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createProductResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createProductResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = productResponse.Result;
        
        var customer = _customerFaker.Generate();
        var createCustomerResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createCustomerResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;

        var orderRequest = new PlaceOrderRequest
        {
            CustomerId = Guid.Parse(customerId),
            ProductId = Guid.Parse(productId)
        };
        var placeOrderResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Order", orderRequest);
        var orderResponse = await placeOrderResponse.Content.ReadFromJsonAsync<Envelope>();
        var orderId = orderResponse.Result;
        
        // Act
        var getOrderDetailsResponse = await _adminHttpClient.GetAsync($"/api/v1/Order/{customerId}/{orderId}/Details");
        
        // Assert
        getOrderDetailsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getOrderDetailsResult = await getOrderDetailsResponse.Content.ReadFromJsonAsync<Envelope<OrderDetailsResponse>>();
        getOrderDetailsResult.Result.CustomerId.Should().Be(Guid.Parse(customerId));
        getOrderDetailsResult.Result.ProductId.Should().Be(Guid.Parse(productId));
        getOrderDetailsResult.Result.Id.Should().Be(Guid.Parse(orderId));
    }
    
    [Fact]
    public async Task GetOrderDetails_ShouldReturnNotFound_WhenOrderDetailsNotExist()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createCustomerResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createCustomerResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;
        
        // Act
        var getOrderDetailsResponse = await _adminHttpClient.GetAsync($"/api/v1/Order/{customerId}/{Guid.NewGuid()}/Details");
        
        // Assert
        getOrderDetailsResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var getOrderDetailsResult = await getOrderDetailsResponse.Content.ReadFromJsonAsync<Envelope>();
        getOrderDetailsResult.Errors["error"][0].Should().Contain("Order not found");
    }
    
    [Fact]
    public async Task GetAllOrders_ShouldReturnOk_WhenOrdersExist()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createProductResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createProductResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = productResponse.Result;
        
        var customer = _customerFaker.Generate();
        var createCustomerResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createCustomerResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;

        var orderRequest = new PlaceOrderRequest
        {
            CustomerId = Guid.Parse(customerId),
            ProductId = Guid.Parse(productId)
        };
        var placeOrderResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Order", orderRequest);
        var orderResponse = await placeOrderResponse.Content.ReadFromJsonAsync<Envelope>();
        var orderId = orderResponse.Result;
        
        // Act
        var getAllOrdersResponse = await _adminHttpClient.GetAsync($"/api/v1/Order/{customerId}/all");
        
        // Assert
        getAllOrdersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getAllOrdersResult = await getAllOrdersResponse.Content.ReadFromJsonAsync<Envelope<List<OrderResponse>>>();
        getAllOrdersResult.Result.First().CustomerId.Should().Be(Guid.Parse(customerId));
        getAllOrdersResult.Result.First().ProductId.Should().Be(Guid.Parse(productId));
        getAllOrdersResult.Result.First().Id.Should().Be(Guid.Parse(orderId));
    }
    
    [Fact]
    public async Task GetAllOrders_ShouldReturnNotFound_WhenOrdersNotExist()
    {
        // Arrange
        var customer = _customerFaker.Generate();
        var createCustomerResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Customer", customer);
        var customerResponse = await createCustomerResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = customerResponse.Result;
        
        // Act
        var getAllOrdersResponse = await _adminHttpClient.GetAsync($"/api/v1/Order/{customerId}/all");
        
        // Assert
        getAllOrdersResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var getAllOrdersResult = await getAllOrdersResponse.Content.ReadFromJsonAsync<Envelope>();
        getAllOrdersResult.Errors["error"][0].Should().Contain("Order not found");
    }
    
    [Fact]
    public async Task GetAllOrders_ShouldReturnBadRequest_WhenCustomerIdIsInvalid()
    {
        // Act
        var getAllOrdersResponse = await _adminHttpClient.GetAsync($"/api/v1/Order/{Guid.NewGuid()}/all");
        
        // Assert
        getAllOrdersResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var getAllOrdersResult = await getAllOrdersResponse.Content.ReadFromJsonAsync<Envelope>();
        getAllOrdersResult.Errors["error"][0].Should().Contain("Customer not found");
    }
    
    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Act
        var getResponse = await _httpClient.GetAsync($"/api/v1/Order/{Guid.NewGuid()}/{Guid.NewGuid()}");
        
        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetOrderDetails_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Act
        var getResponse = await _httpClient.GetAsync($"/api/v1/Order/{Guid.NewGuid()}/{Guid.NewGuid()}/Details");
        
        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task GetAllOrders_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Act
        var getResponse = await _httpClient.GetAsync($"/api/v1/Order/{Guid.NewGuid()}/all");
        
        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}