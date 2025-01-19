namespace BlueFrames.Api.Contracts.Customers.Requests;

public class CustomerRequest
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Phone { get; init; }
    public string Email { get; init; }
}