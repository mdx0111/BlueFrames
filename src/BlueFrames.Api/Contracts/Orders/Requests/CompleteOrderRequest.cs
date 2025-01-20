namespace BlueFrames.Api.Contracts.Orders.Requests;

public record CompleteOrderRequest
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
}