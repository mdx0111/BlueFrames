using BlueFrames.Api.Contracts.Products.Requests;

namespace BlueFrames.Api.Tests.Integration.ProductController;

public class CreateProductControllerTests : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _adminHttpClient;

    private const int ProductSKUCharacterCount = 5;
    private readonly Faker<ProductRequest> _productFaker = new Faker<ProductRequest> ("en_GB")
        .RuleFor(dto => dto.Name, f => f.Commerce.ProductName())
        .RuleFor(dto => dto.Description, f => f.Commerce.ProductDescription())
        .RuleFor(dto => dto.SKU, f => f.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper());

    private readonly HttpClient _userHttpClient;

    public CreateProductControllerTests(BlueFramesApiFactory factory)
    {
        _adminHttpClient = factory.CreateHttpClientWithAdminCredentials();
        _userHttpClient = factory.CreateHttpClientWithUserCredentials();
    }
    
    [Fact]
    public async Task Create_ShouldReturnCreated_WhenProductIsCreated()
    {
        // Arrange
        var product = _productFaker.Generate();

        // Act
        var response = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createResponse = await response.Content.ReadFromJsonAsync<Envelope>();
        createResponse.Should().NotBeNull();
        createResponse.Result.Should().NotBeEmpty();
        createResponse.Result.Should().NotBe(Guid.Empty.ToString());
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenProductIsInvalid()
    {
        // Arrange
        var product = _productFaker.Clone()
            .RuleFor(dto => dto.Name, string.Empty)
            .Generate();

        // Act
        var response = await _adminHttpClient.PostAsJsonAsync("/api/v1/Product", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationError = await response.Content.ReadFromJsonAsync<Envelope>();
        validationError.Errors["error"][0].Should().Contain("is not a valid product name");
    }
    
    [Fact]
    public async Task Create_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        // Arrange
        var product = _productFaker.Generate();

        // Act
        var response = await _userHttpClient.PostAsJsonAsync("/api/v1/Product", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}