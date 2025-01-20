namespace BlueFrames.Api.Contracts.Products.Responses;

public class ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string SKU { get; init; }
}