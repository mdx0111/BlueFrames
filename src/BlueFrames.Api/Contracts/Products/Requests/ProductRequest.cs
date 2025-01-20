namespace BlueFrames.Api.Contracts.Products.Requests;

public record ProductRequest
{
    public string Name { get; init; }
    public string Description { get; init; }
    public string SKU { get; init; }
}