namespace BlueFrames.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
}