namespace BlueFrames.Api.Contracts.Products.Requests;

public record UpdateProductRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; init; }

    [FromBody]
    public ProductRequest Product { get; set; }
}