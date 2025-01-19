using BlueFrames.Application.Products.Commands.CreateProduct;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.App.Tests.Unit.Products;

public class CreateProductTests
{ 
    private readonly Product _product;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILoggerAdapter<CreateProductCommandHandler> _logger = Substitute.For<ILoggerAdapter<CreateProductCommandHandler>>();

    public CreateProductTests()
    {
        var commerce = new Bogus.DataSets.Commerce();
        _product = Product.Create(
            ProductName.From(commerce.ProductName()),
            ProductDescription.From(commerce.ProductDescription()),
            ProductSku.From(commerce.Product()));
    }
    
    [Fact]
    public async Task CreateProduct_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var createProduct = new CreateProductCommand(
            _product.Name.Value,
            _product.Description.Value,
            _product.SKU.Value);

        var handler = new CreateProductCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var result = await handler.Handle(createProduct, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}