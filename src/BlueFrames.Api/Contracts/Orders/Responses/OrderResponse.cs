namespace BlueFrames.Api.Contracts.Orders.Responses;

public record OrderResponse
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public Guid ProductId { get; init; }
    public DateTime OrderDate { get; init; }
    public DateTime UpdateDate { get; init; }
    public string Status { get; init; }
}