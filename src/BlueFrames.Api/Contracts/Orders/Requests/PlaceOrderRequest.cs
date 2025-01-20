namespace BlueFrames.Api.Contracts.Orders.Requests;

public class PlaceOrderRequest
{
    public Guid CustomerId { get; init; }
    public Guid ProductId { get; init; }
}