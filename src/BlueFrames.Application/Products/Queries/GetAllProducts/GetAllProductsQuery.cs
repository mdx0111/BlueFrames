using BlueFrames.Application.Products.Queries.Common;

namespace BlueFrames.Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<Result<List<ProductDto>>>
{
    public int Limit { get; }
    public int Offset { get; }
    
    public GetAllProductsQuery(int limit, int offset)
    {
        Limit = limit;
        Offset = offset;
    }
}