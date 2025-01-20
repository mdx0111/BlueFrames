using BlueFrames.Api.Contracts.Products.Requests;
using BlueFrames.Api.Contracts.Products.Responses;

namespace BlueFrames.Api.Tests.Integration.ProductController;

public class GetProductControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _httpClient;

    private const int ProductSKUCharacterCount = 5;
    private readonly Faker<ProductRequest> _productFaker = new Faker<ProductRequest> ("en_GB")
        .RuleFor(dto => dto.Name, f => f.Commerce.ProductName())
        .RuleFor(dto => dto.Description, f => f.Commerce.ProductDescription())
        .RuleFor(dto => dto.SKU, f => f.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper());
    
    public GetProductControllerTests(BlueFramesApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }
    
    [Fact]
    public async Task Get_ShouldReturnProduct_WhenProductExist()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createResponse = await _httpClient.PostAsJsonAsync("/api/v1/Product", product);
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
        var createResponse = await _httpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var customerId = productResponse.Result;

        // Act
        var response = await _httpClient.GetAsync("/api/v1/Product?offset=0&limit=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var getProductResponse = await response.Content.ReadFromJsonAsync<Envelope<List<ProductResponse>>>();
        getProductResponse.Result.Should().NotBeEmpty();
        getProductResponse.Result.Single().Should().BeEquivalentTo(new ProductResponse
        {
            Id = Guid.Parse(customerId),
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU
        });
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