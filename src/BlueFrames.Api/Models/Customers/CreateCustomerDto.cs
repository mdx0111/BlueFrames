namespace BlueFrames.Api.Models.Customers;

public class CreateCustomerDto
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Phone { get; init; }
    public string Email { get; init; }
}