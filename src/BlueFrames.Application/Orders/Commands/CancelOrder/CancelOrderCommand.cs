using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders.Common;

namespace BlueFrames.Application.Orders.Commands.CancelOrder;

public record CancelOrderCommand : IRequest<Result>
{
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }
    
    public CancelOrderCommand(OrderId orderId, CustomerId customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }
}