namespace BlueFrames.Domain.Products;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string SKU { get; private set; }
    
    protected Product()
    {
    }

    public Product(string name, string description, string sku)
    {
        Name = name;
        Description = description;
        SKU = sku;
    }
}