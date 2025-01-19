using BlueFrames.Application.Common.Results;
using BlueFrames.Application.Products.Queries.Common;
using BlueFrames.Application.Products.Queries.GetAllProducts;
using BlueFrames.Application.Products.Queries.GetProductById;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;
using NSubstitute.ReturnsExtensions;

namespace BlueFrames.App.Tests.Unit.Products;

public class GetProductTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();

    private readonly Bogus.DataSets.Commerce _commerce = new();
    private const int ProductSKUCharacterCount = 5;
    private readonly List<Product> _listOfProducts;

    public GetProductTests()
    {
        _listOfProducts = [];
        
        for (var index = 0; index < 10; index++)
        {
            var product = Product.Create(
                ProductName.From(_commerce.ProductName()),
                ProductDescription.From(_commerce.ProductDescription()),
                ProductSku.From(_commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()));
            _listOfProducts.Add(product);
        }
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnProducts_WhenExists()
    {
        // Arrange
        _repository.GetAllAsync(10, 0, _cancellationToken).Returns(_listOfProducts);
        
        var query = new GetAllProductsQuery(10, 0);
        var logger = Substitute.For<ILoggerAdapter<GetAllProductsQueryHandler>>();
        var handler = new GetAllProductsQueryHandler(_repository, logger);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<List<ProductDto>>>();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<List<ProductDto>>();
        result.Value.Count.Should().Be(_listOfProducts.Count);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnFailure_WhenPageIsEmpty()
    {
        // Arrange
        _repository.GetAllAsync(10, 10, _cancellationToken).Returns([]);
        
        var query = new GetAllProductsQuery(10, 0);
        var logger = Substitute.For<ILoggerAdapter<GetAllProductsQueryHandler>>();
        var handler = new GetAllProductsQueryHandler(_repository, logger);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<List<ProductDto>>>();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
    }
    
    [Fact]
    public async Task GetProduct_ShouldReturnProduct_WhenFound()
    {
        // Arrange
        var firstProduct = _listOfProducts.First();
        _repository.GetByIdAsync(firstProduct.Id.Value, _cancellationToken).Returns(firstProduct);
        
         var query = new GetProductByIdQuery(firstProduct.Id.Value);
         var logger = Substitute.For<ILoggerAdapter<GetProductByIdQueryHandler>>();
         var handler = new GetProductByIdQueryHandler(_repository, logger);

         // Act
         var result = await handler.Handle(query, _cancellationToken);

         // Assert
         result.Should().NotBeNull();
         result.Should().BeOfType<Result<ProductDto>>();
         result.IsSuccess.Should().BeTrue();
         result.Value.Should().BeEquivalentTo(new ProductDto
        {
            Id = firstProduct.Id.Value,
            Name = firstProduct.Name.ToString(),
            Description = firstProduct.Description.ToString(),
            SKU = firstProduct.SKU.ToString()
        });
    }
    
    [Fact]
    public async Task GetProduct_ShouldReturnFailure_WhenNotFound()
    {
        // Arrange
        var firstProduct = _listOfProducts.First();
        _repository.GetByIdAsync(firstProduct.Id.Value, _cancellationToken).ReturnsNull();
        
        var query = new GetProductByIdQuery(firstProduct.Id.Value);
        var logger = Substitute.For<ILoggerAdapter<GetProductByIdQueryHandler>>();
        var handler = new GetProductByIdQueryHandler(_repository, logger);

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Result<ProductDto>>();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}