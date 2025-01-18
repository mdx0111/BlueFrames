using BlueFrames.Domain.Customers;
using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders.Common;
using BlueFrames.Domain.Products;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Domain.Orders;

public class Order : Entity, IAggregateRoot
{
    public OrderId Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public Status Status { get; private set; }
    public OrderDate CreatedDate { get; private set; }
    public OrderDate UpdatedDate { get; private set; }

    public Product Product { get; private set; }
    public Customer Customer { get; private set; }

    public static Order Create(ProductId productId, CustomerId customerId, OrderDate createdDate, DateTime now)
    {
        return new Order(productId, customerId, createdDate, now);
    }

    protected Order()
    {
    }

    private Order(ProductId productId, CustomerId customerId, OrderDate createdDate, DateTime now)
    {
        if (createdDate.Value < now)
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
        UpdatedDate = OrderDate.From(now);
    }
    
    public void Complete(DateTime now)
    {
        if (Status == Status.Complete)
        {
            throw new ValidationException("Order is already completed");
        }
        
        Status = Status.Complete;
        UpdatedDate = OrderDate.From(now);
    }
}