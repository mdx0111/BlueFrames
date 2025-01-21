using BlueFrames.Api.Contracts.Products.Requests;

namespace BlueFrames.Api.Tests.Integration.ProductController;

public class DeleteProductControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private const int ProductSKUCharacterCount = 5;
    private readonly Faker<ProductRequest> _productFaker = new Faker<ProductRequest> ("en_GB")
        .RuleFor(dto => dto.Name, f => f.Commerce.ProductName())
        .RuleFor(dto => dto.Description, f => f.Commerce.ProductDescription())
        .RuleFor(dto => dto.SKU, f => f.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper());

    private readonly HttpClient _adminHttpClient;
    private readonly HttpClient _userHttpClient;
    private readonly HttpClient _httpClient;

    public DeleteProductControllerTests(BlueFramesApiFactory factory)
    {
        _adminHttpClient = factory.CreateHttpClientWithAdminCredentials();
        _userHttpClient = factory.CreateHttpClientWithUserCredentials();
        _httpClient = factory.CreateClient();
    }
    
    [Fact]
    public async Task Delete_ShouldReturnOk_WhenProductExist()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = productResponse.Result;
        
        // Act
        var response = await _adminHttpClient.DeleteAsync($"/api/v1/Product/{productId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Act
        var response = await _adminHttpClient.DeleteAsync($"/api/v1/Product/{Guid.NewGuid()}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var validationError = await response.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("Product not found");
    }
    
    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = productResponse.Result;
        
        // Act
        var response = await _userHttpClient.DeleteAsync($"/api/v1/Product/{productId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createResponse = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = productResponse.Result;
        
        // Act
        var response = await _httpClient.DeleteAsync($"/api/v1/Product/{productId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}