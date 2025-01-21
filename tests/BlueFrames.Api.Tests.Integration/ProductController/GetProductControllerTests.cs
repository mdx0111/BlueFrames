using BlueFrames.Api.Contracts.Products.Requests;
using BlueFrames.Api.Contracts.Products.Responses;

namespace BlueFrames.Api.Tests.Integration.ProductController;

public class GetProductControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private const int ProductSKUCharacterCount = 5;
    private readonly Faker<ProductRequest> _productFaker = new Faker<ProductRequest> ("en_GB")
        .RuleFor(dto => dto.Name, f => f.Commerce.ProductName())
        .RuleFor(dto => dto.Description, f => f.Commerce.ProductDescription())
        .RuleFor(dto => dto.SKU, f => f.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper());

    private readonly HttpClient _adminHttpClient;
    private readonly HttpClient _httpClient;

    public GetProductControllerTests(BlueFramesApiFactory factory)
    {
        _adminHttpClient = factory.CreateHttpClientWithAdminCredentials();
        _httpClient = factory.CreateClient();
    }
    
    [Fact]
    public async Task Get_ShouldReturnProduct_WhenProductExist()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = productResponse.Result;

        // Act
        var response = await _httpClient.GetAsync($"/api/v1/Product/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getProductResponse = await response.Content.ReadFromJsonAsync<Envelope<ProductResponse>>();
        getProductResponse.Result.Should().BeEquivalentTo(new ProductResponse
        {
            Id = Guid.Parse(productId),
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU
        });
    }
    
    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Act
        var response = await _httpClient.GetAsync($"/api/v1/Product/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var validationError = await response.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("Product not found");
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnProducts_WhenProductsExist()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = productResponse.Result;

        // Act
        var response = await _httpClient.GetAsync("/api/v1/Product?offset=0&limit=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getProductResponse = await response.Content.ReadFromJsonAsync<Envelope<List<ProductResponse>>>();
        getProductResponse.Result.Should().NotBeEmpty();
        getProductResponse.Result.Should().ContainEquivalentOf(new ProductResponse
        {
            Id = Guid.Parse(productId),
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU
        });
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnProductsFromCache_WhenRequestedSubsequently()
    {
        // Arrange
        var product = _productFaker.Generate();
        _ = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);

        // Act
        var firstResponse = await _httpClient.GetAsync("/api/v1/Product?offset=0&limit=10");
        await Task.Delay(TimeSpan.FromSeconds(2));
        var secondResponse = await _httpClient.GetAsync("/api/v1/Product?offset=0&limit=10");

        // Assert
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var firstGetProductResponse = await firstResponse.Content.ReadFromJsonAsync<Envelope<List<ProductResponse>>>();
        var firstResponseGeneratedTime = firstGetProductResponse.TimeGenerated;
        firstGetProductResponse.Result.Should().NotBeEmpty();
        
        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var secondResponseGeneratedTime = firstGetProductResponse.TimeGenerated;
        var secondGetProductResponse = await secondResponse.Content.ReadFromJsonAsync<Envelope<List<ProductResponse>>>();
        secondGetProductResponse.Result.Should().NotBeEmpty();

        firstResponseGeneratedTime.Should().Be(secondResponseGeneratedTime);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoProductsExist()
    {
        // Act
        var response = await _httpClient.GetAsync("/api/v1/Product?offset=10&limit=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getProductResponse = await response.Content.ReadFromJsonAsync<Envelope<List<ProductResponse>>>();
        getProductResponse.Result.Should().BeEmpty();
    }
}