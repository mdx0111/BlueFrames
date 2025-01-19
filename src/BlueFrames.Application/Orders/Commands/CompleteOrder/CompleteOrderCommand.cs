using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders.Common;

namespace BlueFrames.Application.Orders.Commands.CompleteOrder;

public record CompleteOrderCommand : IRequest<Result>
{
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }
    
    public CompleteOrderCommand(OrderId orderId, CustomerId customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }
}