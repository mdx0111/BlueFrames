namespace BlueFrames.Api.Contracts.Customers.Responses;

public class CustomerResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Phone { get; init; }
    public string Email { get; init; }
}