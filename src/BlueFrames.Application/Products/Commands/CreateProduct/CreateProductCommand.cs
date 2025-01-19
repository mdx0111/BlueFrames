namespace BlueFrames.Application.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<Result<Guid>>
{
    public string ProductName { get; }
    public string ProductDescription { get; }
    public string ProductSKU { get; }

    public CreateProductCommand(string productName, string productDescription, string productSKU)
    {
        ProductName = productName;
        ProductDescription = productDescription;
        ProductSKU = productSKU;
    }
}