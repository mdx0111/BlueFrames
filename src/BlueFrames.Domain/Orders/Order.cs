using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders.Common;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Domain.Orders;

public class Order
{
    public OrderId Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public Status Status { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }

    public Order(ProductId productId, CustomerId customerId, DateTime createdDate, DateTime now)
    {
        if (createdDate < now)
        {
            throw new ValidationException("Invalid Created Date");
        }

        Id = OrderId.From(GuidProvider.Create());
        ProductId = productId ?? throw new ValidationException("Product Id is required");
        CustomerId = customerId ?? throw new ValidationException("Customer Id is required");
        Status = Status.Pending;
        CreatedDate = createdDate;
    }
    
    public void Cancel(DateTime now)
    {
        if (Status == Status.Cancelled)
        {
            throw new ValidationException("Order is already cancelled");
        }
        
        Status = Status.Cancelled;
        UpdatedDate = now;
    }
    
    public void Complete(DateTime now)
    {
        if (Status == Status.Complete)
        {
            throw new ValidationException("Order is already completed");
        }
        
        Status = Status.Complete;
        UpdatedDate = now;
    }
}