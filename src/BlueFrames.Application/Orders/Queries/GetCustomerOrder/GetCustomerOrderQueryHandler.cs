using BlueFrames.Application.Interfaces.Repositories;
using BlueFrames.Application.Orders.Queries.Common;

namespace BlueFrames.Application.Orders.Queries.GetCustomerOrder;

public class GetCustomerOrderQueryHandler : IRequestHandler<GetCustomerOrderQuery, Result<OrderDto>>
{
    private readonly ICustomerRepository _repository;
    private readonly ILoggerAdapter<GetCustomerOrderQueryHandler> _logger;

    public GetCustomerOrderQueryHandler(
        ICustomerRepository repository,
        ILoggerAdapter<GetCustomerOrderQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<Result<OrderDto>> Handle(GetCustomerOrderQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                return Result.Failure<OrderDto>($"Customer not found");
            }

            var order = customer.FindOrderById(request.OrderId.Value);
            return Result.Success(OrderDto.From(order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer order");
            return Result.Failure<OrderDto>("Failed to get customer order");
        }
    }
}