namespace BlueFrames.Application.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand : IRequest<Result<Guid>>
{
    public Guid CustomerId { get; }
    public Guid ProductId { get; }
    
    public PlaceOrderCommand(Guid customerId, Guid productId)
    {
        CustomerId = customerId;
        ProductId = productId;
    }
}