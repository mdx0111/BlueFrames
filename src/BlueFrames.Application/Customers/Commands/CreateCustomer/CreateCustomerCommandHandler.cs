using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Customers;

namespace BlueFrames.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerAdapter<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork,
        ILoggerAdapter<CreateCustomerCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = Customer.Create(
                request.FirstName,
                request.LastName,
                request.Phone,
                request.Email);
        
            _repository.Add(customer);
        
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
            return Result.Success(customer.Id.Value);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error creating customer");
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return Result.Failure<Guid>(ex.Message);
        }
    }
}