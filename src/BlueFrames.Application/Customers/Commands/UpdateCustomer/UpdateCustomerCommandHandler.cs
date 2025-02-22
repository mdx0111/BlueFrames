using BlueFrames.Application.Interfaces.Repositories;

namespace BlueFrames.Application.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<Guid>>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerAdapter<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork,
        ILoggerAdapter<UpdateCustomerCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                return Result.Failure<Guid>($"Customer with Id {request.Id} not found.");
            }
            
            customer.ChangeFirstName(request.FirstName);
            customer.ChangeLastName(request.LastName);
            customer.ChangePhone(request.Phone);
            customer.ChangeEmail(request.Email);
        
            _repository.Update(customer);
        
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
            return Result.Success(customer.Id.Value);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error updating customer");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer");
            return Result.Failure<Guid>(ex.Message);
        }
    }
}