using BlueFrames.Domain.Orders;

namespace BlueFrames.Application.Orders.Queries.Common;

public record OrderDto
{
    public Guid Id { get; protected set; }
    public Guid CustomerId { get; protected set; }
    public Guid ProductId { get; protected set; }
    public DateTime CreatedDate { get; protected set; }
    public DateTime UpdatedDate { get; protected set; }
    public string Status { get; protected set; }
    
    public static OrderDto From(Order order)
    {
        if (order is null)
        {
            return null;
        }

        return new OrderDto
        {
            Id = order.Id.Value,
            CustomerId = order.CustomerId.Value,
            ProductId = order.ProductId.Value,
            Status = order.Status.ToString(),
            CreatedDate = order.CreatedDate.Value,
            UpdatedDate = order.UpdatedDate?.Value ?? DateTime.MinValue
        };
    }
}