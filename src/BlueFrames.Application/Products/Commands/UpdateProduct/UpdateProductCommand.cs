using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<Result<Guid>>
{
    public ProductId Id { get; set; }
    public ProductName ProductName { get; }
    public ProductDescription ProductDescription { get; }
    public ProductSKU ProductSKU { get; }

    public UpdateProductCommand(
        ProductId id,
        ProductName productName,
        ProductDescription productDescription,
        ProductSKU productSKU)
    {
        Id = id;
        ProductName = productName;
        ProductDescription = productDescription;
        ProductSKU = productSKU;
    }
}