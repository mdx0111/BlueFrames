using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Application.Products.Queries.Common;

namespace BlueFrames.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly ILoggerAdapter<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IProductRepository repository,
        ILoggerAdapter<GetProductByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _repository.GetByIdAsync(
                request.Id.Value,
                cancellationToken);
            
            if (product is null)
            {
                return Result.Failure<ProductDto>("Error retrieving product");
            }
            
            var result = new ProductDto
            {
                Id = product.Id.Value,
                Name = product.Name.ToString(),
                Description = product.Description.ToString(),
                SKU = product.SKU.ToString()
            };
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product");
            return Result.Failure<ProductDto>("Error retrieving product");
        }
    }
}