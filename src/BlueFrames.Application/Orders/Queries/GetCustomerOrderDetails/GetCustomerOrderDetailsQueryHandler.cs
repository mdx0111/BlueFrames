using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Application.Orders.Queries.Common;

namespace BlueFrames.Application.Orders.Queries.GetCustomerOrderDetails;

public class GetCustomerOrderDetailsQueryHandler : IRequestHandler<GetCustomerOrderDetailsQuery, Result<OrderDetailsDto>>
{
    private readonly ICustomerRepository _repository;
    private readonly ILoggerAdapter<GetCustomerOrderDetailsQueryHandler> _logger;

    public GetCustomerOrderDetailsQueryHandler(
        ICustomerRepository repository,
        ILoggerAdapter<GetCustomerOrderDetailsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Result<OrderDetailsDto>> Handle(GetCustomerOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(request.CustomerId.Value, cancellationToken);
            if (customer is null)
            {
                return Result.Failure<OrderDetailsDto>($"Customer with Id {request.CustomerId} was not found");
            }

            var order = customer.FindOrderById(request.OrderId.Value);
            if (order is null)
            {
                return Result.Failure<OrderDetailsDto>($"Order with Id {request.OrderId} was not found");
            }
            
            return Result.Success(OrderDetailsDto.From(order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer order details");
            return Result.Failure<OrderDetailsDto>("Failed to get customer order details");
        }
    }
}