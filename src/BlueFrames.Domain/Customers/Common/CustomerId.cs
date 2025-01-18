namespace BlueFrames.Domain.Customers.Common;

public class CustomerId : ValueOf<Guid, CustomerId>
{
    protected override void Validate()
    {
        if (Value != Guid.Empty)
        {
            return;
        }
        
        const string message = "Customer Id cannot be empty";
        throw new ValidationException(message, [
            new ValidationFailure(nameof(CustomerId), message)
        ]);
    }
}