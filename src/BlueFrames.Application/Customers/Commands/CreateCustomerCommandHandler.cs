using BlueFrames.Application.Interfaces.Common;
using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Application.Customers.Commands;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = Customer.Create(
            FirstName.From(request.FirstName),
            LastName.From(request.LastName),
            PhoneNumber.From(request.Phone),
            Email.From(request.Email));
        
        _repository.AddOrUpdate(customer);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success(customer.Id.Value);
    }
}