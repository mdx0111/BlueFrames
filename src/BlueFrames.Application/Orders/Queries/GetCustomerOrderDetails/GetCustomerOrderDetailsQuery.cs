using BlueFrames.Application.Orders.Queries.Common;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders.Common;

namespace BlueFrames.Application.Orders.Queries.GetCustomerOrderDetails;

public record GetCustomerOrderDetailsQuery : IRequest<Result<OrderDetailsDto>>
{
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }
    
    public GetCustomerOrderDetailsQuery(OrderId orderId, CustomerId customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }
}