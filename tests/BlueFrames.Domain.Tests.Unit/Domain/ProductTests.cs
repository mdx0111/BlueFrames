using BlueFrames.Domain.Products;

namespace BlueFrames.Domain.Tests.Unit.Domain;

public class ProductTests
{
    private readonly Product _product;
    private readonly ProductDetails _productDetails;
    
    public ProductTests()
    {
        var commerce = new Bogus.DataSets.Commerce(locale: "en_GB");

        _productDetails = new ProductDetails(
            Name: commerce.ProductName(),
            Description: commerce.ProductDescription(),
            SKU: commerce.Random.AlphaNumeric(5).ToUpper()
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
}

internal record ProductDetails(string Name, string Description, string SKU);