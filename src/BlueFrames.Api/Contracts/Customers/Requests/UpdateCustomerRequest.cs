namespace BlueFrames.Api.Contracts.Customers.Requests;

public record UpdateCustomerRequest
{
    [FromRoute(Name = "id")] public Guid Id { get; init; }

    [FromBody] public CustomerRequest Customer { get; set; }
}
