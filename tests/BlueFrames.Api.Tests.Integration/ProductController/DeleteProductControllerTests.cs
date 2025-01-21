using BlueFrames.Api.Contracts.Products.Requests;

namespace BlueFrames.Api.Tests.Integration.ProductController;

public class DeleteProductControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _httpClient;

    private const int ProductSKUCharacterCount = 5;
    private readonly Faker<ProductRequest> _productFaker = new Faker<ProductRequest> ("en_GB")
        .RuleFor(dto => dto.Name, f => f.Commerce.ProductName())
        .RuleFor(dto => dto.Description, f => f.Commerce.ProductDescription())
        .RuleFor(dto => dto.SKU, f => f.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper());

    
    public DeleteProductControllerTests(BlueFramesApiFactory factory)
    {
        _httpClient = factory.CreateHttpClientWithAdminCredentials();
    }
    
    [Fact]
    public async Task Delete_ShouldReturnOk_WhenProductExist()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createResponse = await _httpClient.PostAsJsonAsync("/api/v1/Product", product);
        var productResponse = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = productResponse.Result;
        
        // Act
        var response = await _httpClient.DeleteAsync($"/api/v1/Product/{productId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Act
        var response = await _httpClient.DeleteAsync($"/api/v1/Product/{Guid.NewGuid()}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var validationError = await response.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("Product not found");
    }
}