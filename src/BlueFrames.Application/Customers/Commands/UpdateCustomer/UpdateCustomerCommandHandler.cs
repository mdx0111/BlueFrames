using BlueFrames.Application.Interfaces.Common;
using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Application.Customers.Commands.UpdateCustomer;

internal class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<Guid>>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (customer == null)
            {
                return Result.Failure<Guid>($"Customer with Id {request.Id} not found.");
            }
            
            customer.ChangeFirstName(FirstName.From(request.FirstName));
            customer.ChangeLastName(LastName.From(request.LastName));
            customer.ChangePhone(PhoneNumber.From(request.Phone));
            customer.ChangeEmail(Email.From(request.Email));
        
            _repository.AddOrUpdate(customer);
        
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        
            return Result.Success(customer.Id.Value);
        }
        catch (ValidationException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}