using BlueFrames.Domain.Products;

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
            Name: commerce.ProductName(),
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
    
    [Fact]
    public void Create_ShouldThrowException_WhenNameIsNull()
    {
        // Act
        Action act = () => new Product(null, _productDetails.Description, _productDetails.SKU);

        // Assert
        act.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenNameIsEmpty()
    {
        // Act
        Action act = () => new Product(string.Empty, _productDetails.Description, _productDetails.SKU);

        // Assert
        act.Should().Throw<ValidationException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenNameIsInvalid()
    {
        // Act
        Action act = () => new Product(" ", _productDetails.Description, _productDetails.SKU);

        // Assert
        act.Should().Throw<ValidationException>();
    }
    
}

internal record ProductDetails(string Name, string Description, string SKU);