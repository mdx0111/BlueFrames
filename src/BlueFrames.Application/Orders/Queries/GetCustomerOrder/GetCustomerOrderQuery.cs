using BlueFrames.Application.Orders.Queries.Common;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders.Common;

namespace BlueFrames.Application.Orders.Queries.GetCustomerOrder;

public record GetCustomerOrderQuery : IRequest<Result<OrderDto>>
{
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }
    
    public GetCustomerOrderQuery(OrderId orderId, CustomerId customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }
}