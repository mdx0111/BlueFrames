namespace BlueFrames.Application.Products.Queries.Common;

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string SKU { get; init; }
}