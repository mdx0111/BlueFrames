namespace BlueFrames.Api.Contracts.Orders.Responses;

public record OrderDetailsResponse : OrderResponse
{
    public string CustomerFirstName { get; init; }
    public string CustomerLastName { get; init; }
    public string ProductName { get; init; }
}