namespace BlueFrames.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<Result<Guid>>
{
    public Guid CustomerId { get; }
    public Guid ProductId { get; }
    
    public CreateOrderCommand(Guid customerId, Guid productId)
    {
        CustomerId = customerId;
        ProductId = productId;
    }
}