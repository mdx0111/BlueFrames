using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Domain.Products;

public class Product
{
    public ProductId Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string SKU { get; private set; }
    
    protected Product()
    {
    }

    public Product(string name, string description, string sku)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Name is required");
        }

        var isNameValid = !string.IsNullOrWhiteSpace(name)
                          && name.Length is >= 3 and <= 50
                          && Regex.IsMatch(name, @"^[a-zA-Z0-9\s\-\\_]{3,50}$");
        if (!isNameValid)
        {
            throw new ValidationException($"{name} is not a valid product name");
        }
        
        Id = ProductId.From(GuidProvider.Create());
        Name = name;
        Description = description;
        SKU = sku;
    }
}