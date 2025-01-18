namespace BlueFrames.Domain.Orders;

public class Order
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Status Status { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }

    public Order(Guid productId, Guid customerId, DateTime createdDate)
    {
        ProductId = productId;
        CustomerId = customerId;
        CreatedDate = createdDate;
    }
}