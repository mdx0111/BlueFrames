using BlueFrames.Domain.Orders;

namespace BlueFrames.Application.Orders.Queries.Common;

public record OrderDto
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ProductId { get; private set; }
    
    public static OrderDto From(Order order)
    {
        return new OrderDto
        {
            Id = order.Id.Value,
            CustomerId = order.CustomerId.Value,
            ProductId = order.ProductId.Value
        };
    }
}