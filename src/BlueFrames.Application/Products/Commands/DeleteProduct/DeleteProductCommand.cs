using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand : IRequest<Result>
{
    public ProductId Id { get; set; }

    public DeleteProductCommand(ProductId id)
    {
        Id = id;
    }
}