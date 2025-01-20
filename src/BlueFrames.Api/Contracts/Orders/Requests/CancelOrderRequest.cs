namespace BlueFrames.Api.Contracts.Orders.Requests;

public record CancelOrderRequest
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
}