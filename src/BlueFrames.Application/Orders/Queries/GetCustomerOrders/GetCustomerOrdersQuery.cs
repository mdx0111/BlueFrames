using BlueFrames.Application.Orders.Queries.Common;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders.Common;

namespace BlueFrames.Application.Orders.Queries.GetCustomerOrders;

public record GetCustomerOrdersQuery : IRequest<Result<List<OrderDto>>>
{
    public CustomerId CustomerId { get; }
    
    public GetCustomerOrdersQuery(CustomerId customerId)
    {
        CustomerId = customerId;
    }
}