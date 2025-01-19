using BlueFrames.Application.Products.Commands.UpdateProduct;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.App.Tests.Unit.Products;

public class UpdateProductTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILoggerAdapter<UpdateProductCommandHandler> _logger = Substitute.For<ILoggerAdapter<UpdateProductCommandHandler>>();

    private readonly Bogus.DataSets.Commerce _commerce = new();
    private readonly Product _product;
    private const int ProductSKUCharacterCount = 5;

    public UpdateProductTests()
    {
        _product = Product.Create(
            ProductName.From(_commerce.ProductName()),
            ProductDescription.From(_commerce.ProductDescription()),
            ProductSKU.From(_commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()));

        _repository.GetByIdAsync(_product.Id.Value, _cancellationToken).Returns(_product);
    }
    
    [Fact]
    public async Task UpdateCustomer_ShouldSuccess_WhenGivenValidData()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var updateProduct = new UpdateProductCommand(
            _product.Id,
            ProductName.From(_commerce.ProductName()),
            ProductDescription.From(_commerce.ProductDescription()),
            ProductSKU.From(_commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()));
        var updateHandler = new UpdateProductCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var updateResult = await updateHandler.Handle(updateProduct, _cancellationToken);
        
        // Assert
        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.Should().NotBeEmpty();
        updateResult.Value.Should().Be(_product.Id.Value);
        
        _product.Name.Should().Be(updateProduct.ProductName);
        _product.Description.Should().Be(updateProduct.ProductDescription);
        _product.SKU.Should().Be(updateProduct.ProductSKU);
    }
    
    [Fact]
    public async Task UpdateCustomer_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange
        _unitOfWork.SaveChangesAsync(_cancellationToken).Returns(1);
        
        var updateProduct = new UpdateProductCommand(
            ProductId.From(Guid.NewGuid()),
            ProductName.From(_commerce.ProductName()),
            ProductDescription.From(_commerce.ProductDescription()),
            ProductSKU.From(_commerce.Random.AlphaNumeric(ProductSKUCharacterCount).ToUpper()));
        var updateHandler = new UpdateProductCommandHandler(_repository, _unitOfWork, _logger);
        
        // Act
        var updateResult = await updateHandler.Handle(updateProduct, _cancellationToken);
        
        // Assert
        updateResult.IsSuccess.Should().BeFalse();
    }
}