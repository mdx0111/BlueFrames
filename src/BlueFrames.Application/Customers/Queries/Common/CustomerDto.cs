using BlueFrames.Domain.Customers;

namespace BlueFrames.Application.Customers.Queries.Common;

public record CustomerDto
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    
    public static CustomerDto From(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id.Value,
            FirstName = customer.FirstName.ToString(),
            LastName = customer.LastName.ToString(),
            Phone = customer.Phone.ToString(),
            Email = customer.Email.ToString()
        };
    }
}