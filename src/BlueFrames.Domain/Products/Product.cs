using BlueFrames.Domain.Orders;
using BlueFrames.Domain.Products.Common;

namespace BlueFrames.Domain.Products;

public class Product : Entity, IAggregateRoot
{
    public ProductId Id { get; private set; }
    public ProductName Name { get; private set; }
    public ProductDescription Description { get; private set; }
    public ProductSKU SKU { get; private set; }
    public bool IsDeleted { get; private set; }
    
    private readonly List<Order> _orders = [];
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    
    public static Product Create(ProductName name, ProductDescription description, ProductSKU sku)
    {
        return new Product(name, description, sku);
    }
    
    protected Product()
    {
    }

    private Product(ProductName name, ProductDescription description, ProductSKU sku)
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
    
    public void ChangeSKU(ProductSKU sku)
    {
        SKU = sku ?? throw new ValidationException("SKU is required");
    }

    public void Deactivate()
    {
        IsDeleted = true;
    }
}