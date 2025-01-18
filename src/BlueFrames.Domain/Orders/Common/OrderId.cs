namespace BlueFrames.Domain.Orders.Common;

public class OrderId : ValueOf<Guid, OrderId>
{
    protected override void Validate()
    {
        if (Value != Guid.Empty)
        {
            return;
        }
        
        const string message = "Order Id cannot be empty";
        throw new ValidationException(message, [
            new ValidationFailure(nameof(OrderId), message)
        ]);
    }
}