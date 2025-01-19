namespace BlueFrames.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string ProductName { get; }
    public string ProductDescription { get; }
    public string ProductSKU { get; }

    public UpdateProductCommand(
        Guid id,
        string productName,
        string productDescription,
        string productSKU)
    {
        Id = id;
        ProductName = productName;
        ProductDescription = productDescription;
        ProductSKU = productSKU;
    }
}