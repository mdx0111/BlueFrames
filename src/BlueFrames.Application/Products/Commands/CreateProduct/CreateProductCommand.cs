using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Application.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<Result<Guid>>
{
    public ProductName ProductName { get; }
    public ProductDescription ProductDescription { get; }
    public ProductSKU ProductSKU { get; }

    public CreateProductCommand(
        ProductName productName,
        ProductDescription productDescription,
        ProductSKU productSKU)
    {
        ProductName = productName;
        ProductDescription = productDescription;
        ProductSKU = productSKU;
    }
}