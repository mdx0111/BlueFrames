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
    
    public void ChangeName(ProductName name)
    {
        Name = name ?? throw new ValidationException("Name is required");
    }
    
    public void ChangeDescription(ProductDescription description)
    {
        Description = description ?? throw new ValidationException("Description is required");
    }
    
    public void ChangeSKU(ProductSku sku)
    {
        SKU = sku ?? throw new ValidationException("SKU is required");
    }
}