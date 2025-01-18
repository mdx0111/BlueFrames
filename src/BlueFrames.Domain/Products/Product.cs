using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Domain.Products;

public class Product
{
    public ProductId Id { get; private set; }
    public ProductName Name { get; private set; }
    public string Description { get; private set; }
    public string SKU { get; private set; }
    
    protected Product()
    {
    }

    public Product(ProductName name, string description, string sku)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ValidationException("Description is required");
        }
        
        if (Regex.IsMatch(description, @"^[\p{L}0-9.,\-()%'& ]{10,200}$") == false)
        {
            throw new ValidationException("Description is invalid");
        }
        
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ValidationException("SKU is required");
        }
        
        if (Regex.IsMatch(sku, "^[a-zA-Z0-9]{5}$") == false)
        {
            throw new ValidationException("SKU is invalid");
        }
        
        Id = ProductId.From(GuidProvider.Create());
        Name = name ?? throw new ValidationException("Name is required");
        Description = description;
        SKU = sku;
    }
}