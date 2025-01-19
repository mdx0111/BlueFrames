using BlueFrames.Application.Interfaces.Repositories;

namespace BlueFrames.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerAdapter<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork,
        ILoggerAdapter<UpdateProductCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _repository.GetByIdAsync(request.Id.Value, cancellationToken);
            if (product is null)
            {
                return Result.Failure<Guid>($"Customer with Id {request.Id} not found.");
            }
            
            product.ChangeName(request.ProductName);
            product.ChangeDescription(request.ProductDescription);
            product.ChangeSKU(request.ProductSKU);
        
            _repository.AddOrUpdate(product);
        
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
            return Result.Success(product.Id.Value);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error updating product");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product");
            return Result.Failure<Guid>(ex.Message);
        }
    }
}