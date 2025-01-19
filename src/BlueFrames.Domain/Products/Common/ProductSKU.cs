namespace BlueFrames.Domain.Products.Common;

public partial class ProductSKU : ValueOf<string, ProductSKU>
{
    [GeneratedRegex("^[a-zA-Z0-9]{5}$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex CompiledProductSkuRegex();
    
    private static readonly Regex ProductSkuRegex = CompiledProductSkuRegex();

    protected override void Validate()
    {
        var isNameValid = !string.IsNullOrWhiteSpace(Value)
                          && ProductSkuRegex.IsMatch(Value);
        if (isNameValid)
        {
            return;
        }

        var message = $"{Value} is not a valid product sku";
        throw new ValidationException($"{Value} is not a valid product sku", [
            new ValidationFailure(nameof(ProductSKU), message)
        ]);
    }
}