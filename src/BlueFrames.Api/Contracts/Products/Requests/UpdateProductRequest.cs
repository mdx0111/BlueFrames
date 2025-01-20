namespace BlueFrames.Api.Contracts.Products.Requests;

public class UpdateProductRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; init; }

    [FromBody]
    public ProductRequest Product { get; set; }
}