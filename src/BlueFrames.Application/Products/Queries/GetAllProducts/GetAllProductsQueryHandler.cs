using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Application.Products.Queries.Common;

namespace BlueFrames.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<List<ProductDto>>>
{
    private readonly IProductRepository _repository;
    private readonly ILoggerAdapter<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(
        IProductRepository repository,
        ILoggerAdapter<GetAllProductsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Result<List<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _repository.GetAllAsync(
                request.Limit,
                request.Offset,
                cancellationToken);
            
            if (products is null)
            {
                return Result.Failure<List<ProductDto>>("Error retrieving products");
            }
            
            if (products.Count == 0)
            {
                return Result.Failure<List<ProductDto>>("No products found");
            }
            
            var result = products.Select(product => new ProductDto
            {
                Id = product.Id.Value,
                Name = product.Name.ToString(),
                Description = product.Description.ToString(),
                SKU = product.SKU.ToString()
            }).ToList();
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return Result.Failure<List<ProductDto>>("Error retrieving products");
        }
    }
}