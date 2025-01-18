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
        
        ProductId = productId;
        CustomerId = customerId;
        Status = Status.Pending;
        CreatedDate = createdDate;
    }
    
    public void UpdateStatus(Status status, DateTime now)
    {
        if (status == this.Status)
        {
            throw new ValidationException("Invalid Status");
        }
        
        Status = status;
        UpdatedDate = now;
    }
}