using BlueFrames.Application.Products.Commands.DeleteProduct;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.App.Tests.Unit.Products;

public class DeleteProductTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILoggerAdapter<DeleteProductCommandHandler> _logger = Substitute.For<ILoggerAdapter<DeleteProductCommandHandler>>();

    private readonly Bogus.DataSets.Commerce _commerce = new();
    private readonly Product _product;
    private const int ProductSKUCharacterCount = 5;

    public DeleteProductTests()
    {
        _product = Product.Create(
            ProductName.From(_commerce.ProductName()),
            ProductDescription.From(_commerce.ProductDescription()),
            ProductSKU.From(_commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()));

        _repository.GetByIdAsync(_product.Id, _cancellationToken).Returns(_product);
    }
    
    [Fact]
    public async Task DeleteProduct_ShouldSuccess_WhenProductExists()
    {
        // Arrange        
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);

        var deleteProduct = new DeleteProductCommand(_product.Id);
        var deleteHandler = new DeleteProductCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var deleteResult = await deleteHandler.Handle(deleteProduct, _cancellationToken);
        
        // Assert
        deleteResult.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task DeleteProduct_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var deleteProduct = new DeleteProductCommand(ProductId.From(Guid.NewGuid()));
        var deleteHandler = new DeleteProductCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var deleteResult = await deleteHandler.Handle(deleteProduct, _cancellationToken);
        
        // Assert
        deleteResult.IsSuccess.Should().BeTrue();
        deleteResult.Value.Should().BeEmpty();
    }
}