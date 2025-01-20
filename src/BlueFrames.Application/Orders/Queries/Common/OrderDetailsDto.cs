using BlueFrames.Domain.Orders;

namespace BlueFrames.Application.Orders.Queries.Common;

public record OrderDetailsDto : OrderDto
{
    public string CustomerFirstName { get; private set; }
    public string CustomerLastName { get; private set; }
    public string ProductName { get; private set; }
    
    public new static OrderDetailsDto From(Order order)
    {
        if (order is null)
        {
            return null;
        }
        
        return new OrderDetailsDto
        {
            Id = order.Id.Value,
            CustomerId = order.CustomerId.Value,
            ProductId = order.ProductId.Value,
            Status = order.Status.ToString(),
            CreatedDate = order.CreatedDate.Value,
            UpdatedDate = order.UpdatedDate?.Value ?? DateTime.MinValue,
            CustomerFirstName = order.Customer?.FirstName.ToString(),
            CustomerLastName = order.Customer?.LastName.ToString(),
            ProductName = order.Product?.Name.ToString()
        };
    }
}