namespace BlueFrames.Application.Orders.Commands.CompleteOrder;

public record CompleteOrderCommand : IRequest<Result>
{
    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    
    public CompleteOrderCommand(Guid orderId, Guid customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }
}