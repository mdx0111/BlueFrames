namespace BlueFrames.Domain.Products.Common;

public class ProductId : ValueOf<Guid, ProductId>
{
    protected override void Validate()
    {
        if (Value != Guid.Empty)
        {
            return;
        }
        
        const string message = "Product Id cannot be empty";
        throw new ValidationException(message, [
            new ValidationFailure(nameof(ProductId), message)
        ]);
    }
}