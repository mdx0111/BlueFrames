using BlueFrames.Api.Contracts.Products.Requests;

namespace BlueFrames.Api.Tests.Integration.ProductController;

public class UpdateProductControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _httpClient;

    private const int ProductSKUCharacterCount = 5;
    private readonly Faker<ProductRequest> _productFaker = new Faker<ProductRequest> ("en_GB")
        .RuleFor(dto => dto.Name, f => f.Commerce.ProductName())
        .RuleFor(dto => dto.Description, f => f.Commerce.ProductDescription())
        .RuleFor(dto => dto.SKU, f => f.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper());
    
    public UpdateProductControllerTests(BlueFramesApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenProductIsUpdated()
    {
        // Arrange
        var product = _productFaker.Generate();
        var createResponse = await _httpClient.PostAsJsonAsync("/api/v1/Product", product);
        var createResponseContent = await createResponse.Content.ReadFromJsonAsync<Envelope>();
        var productId = createResponseContent.Result;

        var updatedProduct = _productFaker.Generate();

        // Act
        var updateResponse = await _httpClient.PutAsJsonAsync($"/api/v1/Product/{productId}", updatedProduct);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}