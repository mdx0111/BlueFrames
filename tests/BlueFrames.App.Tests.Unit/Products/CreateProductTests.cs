using BlueFrames.Application.Products.Commands.CreateProduct;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.App.Tests.Unit.Products;

public class CreateProductTests
{ 
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILoggerAdapter<CreateProductCommandHandler> _logger = Substitute.For<ILoggerAdapter<CreateProductCommandHandler>>();
 
    private readonly Product _product;
    private const int ProductSKUCharacterCount = 5;

    public CreateProductTests()
    {
        var commerce = new Bogus.DataSets.Commerce();
        _product = Product.Create(
            ProductName.From(commerce.ProductName()),
            ProductDescription.From(commerce.ProductDescription()),
            ProductSKU.From(commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()));
    }
    
    [Fact]
    public async Task CreateProduct_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var createProduct = new CreateProductCommand(
            _product.Name,
            _product.Description,
            _product.SKU);

        var handler = new CreateProductCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var result = await handler.Handle(createProduct, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}