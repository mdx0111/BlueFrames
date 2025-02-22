namespace BlueFrames.Api.Contracts.Orders.Requests;

public record PlaceOrderRequest
{
    public Guid CustomerId { get; init; }
    public Guid ProductId { get; init; }
}