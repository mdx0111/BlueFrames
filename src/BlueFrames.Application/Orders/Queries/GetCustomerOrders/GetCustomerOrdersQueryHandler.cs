using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Application.Orders.Queries.Common;

namespace BlueFrames.Application.Orders.Queries.GetCustomerOrders;

public class GetCustomerOrdersQueryHandler : IRequestHandler<GetCustomerOrdersQuery, Result<List<OrderDto>>>
{
    private readonly ICustomerRepository _repository;
    private readonly ILoggerAdapter<GetCustomerOrdersQueryHandler> _logger;

    public GetCustomerOrdersQueryHandler(
        ICustomerRepository repository,
        ILoggerAdapter<GetCustomerOrdersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Result<List<OrderDto>>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(request.CustomerId.Value, cancellationToken);
            if (customer == null)
            {
                return Result.Failure<List<OrderDto>>($"Customer with Id {request.CustomerId} was not found");
            }

            var orders = customer
                .Orders
                .Select(OrderDto.From)
                .ToList();
            
            return Result.Success(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer orders");
            return Result.Failure<List<OrderDto>>("Failed to get customer orders");
        }
    }
}