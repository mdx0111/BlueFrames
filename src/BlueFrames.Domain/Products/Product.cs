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
        Id = ProductId.From(GuidProvider.Create());
        Name = name;
        Description = description;
        SKU = sku;
    }
}