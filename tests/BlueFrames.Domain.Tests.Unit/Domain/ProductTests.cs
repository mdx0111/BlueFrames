using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class ProductTests
{
    private readonly Product _product;
    private readonly ProductDetails _productDetails;
    private const int ProductSKUCharacterCount = 5;
    
    public ProductTests()
    {
        var commerce = new Bogus.DataSets.Commerce(locale: "en_GB");

        _productDetails = new ProductDetails(
            Name: ProductName.From(commerce.ProductName()),
            Description: commerce.ProductDescription(),
            SKU: commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()
        );

        _product = new Product(_productDetails.Name, _productDetails.Description, _productDetails.SKU);
    }

    [Fact]
    public void Create_ShouldInitialiseProductWithValidDetails()
    {
        // Assert
        _product.Name.Should().Be(_productDetails.Name);
        _product.Description.Should().Be(_productDetails.Description);
        _product.SKU.Should().Be(_productDetails.SKU);
    }
    
    [Fact]
    public void Create_ShouldInitialiseProductWithValidId()
    {
        //Assert
        _product.Id.Value.Should().NotBeEmpty();
    }
    
    [Theory]
    [InlineData("a")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
    [InlineData("aaa$")]
    [InlineData("")]
    [InlineData(null)]
    public void Create_ShouldThrowException_WhenNameIsInvalid(string productName)
    {
        // Act
        Action act = () => _ = new Product(ProductName.From(productName), _productDetails.Description, _productDetails.SKU);

        // Assert
        act.Should().Throw<ValidationException>();
    }
}

internal record ProductDetails(ProductName Name, string Description, string SKU);