using BlueFrames.Application.Products.Queries.Common;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery : IRequest<Result<ProductDto>>
{
    public ProductId Id { get; }
    
    public GetProductByIdQuery(ProductId id)
    {
        Id = id;
    }
}