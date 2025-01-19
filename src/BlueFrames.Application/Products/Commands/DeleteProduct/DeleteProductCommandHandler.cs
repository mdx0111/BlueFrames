using BlueFrames.Application.Interfaces.Repositories;

namespace BlueFrames.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerAdapter<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork,
        ILoggerAdapter<DeleteProductCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _repository.GetByIdAsync(request.Id.Value, cancellationToken);
            if (product is null)
            {
                return Result.Failure($"Product with Id {request.Id} not found.");
            }

            product.Deactivate();
            _repository.AddOrUpdate(product);
        
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
            return Result.Success();
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error deleting product");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product");
            return Result.Failure<Guid>(ex.Message);
        }
    }
}