using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Domain.Customers;

public class Customer
{
    public CustomerId Id { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public Email Email { get; private set; }

    public Customer(FirstName firstName, LastName lastName, PhoneNumber phone, Email email)
    {
        Id = CustomerId.From(GuidProvider.Create());
        FirstName = firstName ?? throw new ValidationException("First name is required");
        LastName = lastName ??  throw new ValidationException("Last name is required");
        Phone = phone ?? throw new ValidationException("Phone number is required");
        Email = email ?? throw new ValidationException("Email is required");
    }
}