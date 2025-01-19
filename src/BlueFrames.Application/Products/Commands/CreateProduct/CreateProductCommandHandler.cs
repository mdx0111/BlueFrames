using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Products;

namespace BlueFrames.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerAdapter<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork,
        ILoggerAdapter<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = Product.Create(
                request.ProductName,
                request.ProductDescription,
                request.ProductSKU);
        
            _repository.Add(product);
        
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
            return Result.Success(product.Id.Value);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error creating product");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return Result.Failure<Guid>(ex.Message);
        }
    }
}