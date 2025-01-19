namespace BlueFrames.Api.Models.Customers;

public class UpdateCustomerRequest
{
    [FromRoute(Name = "id")] public Guid Id { get; init; }

    [FromBody] public CustomerRequest Customer { get; set; }
}
