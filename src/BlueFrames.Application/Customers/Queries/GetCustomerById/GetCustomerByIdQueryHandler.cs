using BlueFrames.Application.Customers.Queries.Common;
using BlueFrames.Application.Interfaces.Repositories;

namespace BlueFrames.Application.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerRepository _repository;
    private readonly ILoggerAdapter<GetCustomerByIdQueryHandler> _logger;

    public GetCustomerByIdQueryHandler(
        ICustomerRepository repository,
        ILoggerAdapter<GetCustomerByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);
            
            var result = CustomerDto.From(customer);
            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer");
            return Result.Failure<CustomerDto>("Error retrieving customer");
        }
    }
}