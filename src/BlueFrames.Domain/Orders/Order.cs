namespace BlueFrames.Domain.Orders;

public class Order
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Status Status { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }

    public Order(Guid productId, Guid customerId, DateTime createdDate, DateTime now)
    {
        if (productId == Guid.Empty)
        {
            throw new ValidationException("Invalid Product ID");
        }
        
        if (customerId == Guid.Empty)
        {
            throw new ValidationException("Invalid Customer ID");
        }
        
        if (createdDate < now)
        {
            throw new ValidationException("Invalid Created Date");
        }

        Id = GuidProvider.Create();
        ProductId = productId;
        CustomerId = customerId;
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