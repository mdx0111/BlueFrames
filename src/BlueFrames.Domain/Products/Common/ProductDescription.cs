namespace BlueFrames.Domain.Products.Common;

public partial class ProductDescription : ValueOf<string, ProductDescription>
{
    [GeneratedRegex(@"^[\p{L}0-9.,\-()%'& ]{10,200}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex CompiledProductDescriptionRegex();
    
    private static readonly Regex ProductDescriptionRegex = CompiledProductDescriptionRegex();

    protected override void Validate()
    {
        var isNameValid = !string.IsNullOrWhiteSpace(Value)
                          && ProductDescriptionRegex.IsMatch(Value);
        if (!isNameValid)
        {
            throw new ValidationException($"{Value} is not a valid product description");
        }
    }
}