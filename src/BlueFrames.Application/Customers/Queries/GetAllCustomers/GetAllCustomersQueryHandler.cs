using BlueFrames.Application.Customers.Queries.Common;
using BlueFrames.Application.Interfaces.Repositories;

namespace BlueFrames.Application.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<List<CustomerDto>>>
{
    private readonly ICustomerRepository _repository;
    private readonly ILoggerAdapter<GetAllCustomersQueryHandler> _logger;

    public GetAllCustomersQueryHandler(
        ICustomerRepository repository,
        ILoggerAdapter<GetAllCustomersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Result<List<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customers = await _repository.GetAllAsync(
                request.Limit,
                request.Offset,
                cancellationToken);
            
            if (customers is null || customers.Count == 0)
            {
                return Result.Success<List<CustomerDto>>([]);
            }
            
            var result = customers
                .Select(CustomerDto.From)
                .ToList();
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            return Result.Failure<List<CustomerDto>>("Error retrieving customers");
        }
    }
}