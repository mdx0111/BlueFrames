using BlueFrames.Application.Interfaces.Repositories;

namespace BlueFrames.Application.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerAdapter<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork,
        ILoggerAdapter<DeleteCustomerCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                return Result.Failure($"Customer with Id {request.Id} not found.");
            }

            customer.Deactivate();
            _repository.AddOrUpdate(customer);
        
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
            return Result.Success(customer.Id.Value);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error deleting customer");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer");
            return Result.Failure<Guid>(ex.Message);
        }
    }
}