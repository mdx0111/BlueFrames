using BlueFrames.Application.Products.Queries.Common;

namespace BlueFrames.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery : IRequest<Result<ProductDto>>
{
    public Guid Id { get; }
    
    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }
}