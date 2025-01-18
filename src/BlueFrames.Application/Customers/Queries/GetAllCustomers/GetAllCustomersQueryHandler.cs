using BlueFrames.Application.Customers.Queries.Common;
using BlueFrames.Application.Interfaces.Repositories;

namespace BlueFrames.Application.Customers.Queries.GetAllCustomers;

internal class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<List<CustomerDto>>>
{
    private readonly ICustomerRepository _repository;

    public GetAllCustomersQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Result<List<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customers = await _repository.GetAllAsync(
                request.Limit,
                request.Offset,
                cancellationToken);
            
            if (customers is null)
            {
                return Result.Failure<List<CustomerDto>>("Error retrieving customers");
            }
            
            if (customers.Count == 0)
            {
                return Result.Failure<List<CustomerDto>>("No customers found");
            }
            
            var result = customers.Select(customer => new CustomerDto
            {
                Id = customer.Id.Value,
                FirstName = customer.FirstName.ToString(),
                LastName = customer.LastName.ToString(),
                Phone = customer.Phone.ToString(),
                Email = customer.Email.ToString()
            }).ToList();
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<CustomerDto>>("Error retrieving customers");
        }
    }
}