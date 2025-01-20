using BlueFrames.Domain.Products;

namespace BlueFrames.Application.Products.Queries.Common;

public record ProductDto
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string SKU { get; private set; }
    
    public static ProductDto From(Product product)
    {
        if (product is null)
        {
            return null;
        }
        
        return new ProductDto
        {
            Id = product.Id.Value,
            Name = product.Name.ToString(),
            Description = product.Description.ToString(),
            SKU = product.SKU.ToString()
        };
    }
}