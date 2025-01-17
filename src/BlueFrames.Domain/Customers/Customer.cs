namespace BlueFrames.Domain.Customers;

public class Customer
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }

    public Customer(string firstName, string lastName, string phone, string email)
    {
        Id = GuidProvider.Create();
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
    }
}