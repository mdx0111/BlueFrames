namespace BlueFrames.Domain.Products.Common;

public partial class ProductName : ValueOf<string, ProductName>
{
    [GeneratedRegex(@"^[a-zA-Z0-9\s\-\\_]{3,50}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex CompiledProductNameRegex();
    
    private static readonly Regex ProductNameRegex = CompiledProductNameRegex();

    protected override void Validate()
    {
        var isNameValid = !string.IsNullOrWhiteSpace(Value)
                          && ProductNameRegex.IsMatch(Value);
        if (isNameValid)
        {
            return;
        }

        var message = $"{Value} is not a valid product name";
        throw new ValidationException($"{Value} is not a valid product name", [
            new ValidationFailure(nameof(ProductName), message)
        ]);
    }
}