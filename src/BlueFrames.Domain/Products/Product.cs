using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Domain.Products;

public class Product
{
    public ProductId Id { get; private set; }
    public ProductName Name { get; private set; }
    public ProductDescription Description { get; private set; }
    public ProductSku SKU { get; private set; }
    
    protected Product()
    {
    }

    public Product(ProductName name, ProductDescription description, ProductSku sku)
    {
        Id = ProductId.From(GuidProvider.Create());
        Name = name ?? throw new ValidationException("Name is required");
        Description = description ?? throw new ValidationException("Description is required");
        SKU = sku ?? throw new ValidationException("SKU is required");
    }
}