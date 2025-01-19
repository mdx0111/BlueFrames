using BlueFrames.Domain.Orders;

namespace BlueFrames.Application.Orders.Queries.Common;

public record OrderDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public Guid ProductId { get; init; }
    
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