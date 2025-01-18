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
            Description: ProductDescription.From(commerce.ProductDescription()),
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
    [MemberData(nameof(InvalidProductNames))]
    public void Create_ShouldThrowException_WhenNameIsInvalid(string productName)
    {
        // Act
        Action createProduct = () => _ = new Product(ProductName.From(productName), _productDetails.Description, _productDetails.SKU);

        // Assert
        createProduct.Should().Throw<ValidationException>();
    }

    [Theory]
    [MemberData(nameof(InvalidProductDescriptions))]
    public void Create_ShouldThrowException_WhenDescriptionIsInvalid(string productDescription)
    {
        // Act
        Action createProduct = () => _ = new Product(_productDetails.Name, ProductDescription.From(productDescription), _productDetails.SKU);

        // Assert
        createProduct.Should().Throw<ValidationException>();
    }
    
    [Theory]
    [MemberData(nameof(InvalidProductSkus))]
    public void Create_ShouldThrowException_WhenSkuIsInvalid(string sku)
    {
        // Act
        Action createProduct = () => _ = new Product(_productDetails.Name, _productDetails.Description, sku);

        // Assert
        createProduct.Should().Throw<ValidationException>();
    }

    public static TheoryData<string> InvalidProductNames =>
    [
        "a",
        new('a', 51),
        "aaa$",
        "",
        null
    ];
    
    public static TheoryData<string> InvalidProductDescriptions =>
    [
        "a",
        new('a', 201),
        "",
        null
    ];
    
    public static TheoryData<string> InvalidProductSkus =>
    [
        "a",
        new('a', 6),
        "aaa$",
        "",
        null
    ];
}

internal record ProductDetails(ProductName Name, ProductDescription Description, string SKU);