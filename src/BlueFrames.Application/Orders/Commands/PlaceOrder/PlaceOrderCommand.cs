using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Application.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand : IRequest<Result<Guid>>
{
    public CustomerId CustomerId { get; }
    public ProductId ProductId { get; }
    
    public PlaceOrderCommand(CustomerId customerId, ProductId productId)
    {
        CustomerId = customerId;
        ProductId = productId;
    }
}