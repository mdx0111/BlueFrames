using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Domain.Customers;

public class Customer
{
    public CustomerId Id { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public Email Email { get; private set; }

    public static Customer Create(FirstName firstName, LastName lastName, PhoneNumber phone, Email email)
    {
        return new Customer(firstName, lastName, phone, email);
    }
    
    protected Customer()
    {
    }
    
    private Customer(FirstName firstName, LastName lastName, PhoneNumber phone, Email email)
    {
        Id = CustomerId.From(GuidProvider.Create());
        FirstName = firstName ?? throw new ValidationException("First name is required");
        LastName = lastName ??  throw new ValidationException("Last name is required");
        Phone = phone ?? throw new ValidationException("Phone number is required");
        Email = email ?? throw new ValidationException("Email is required");
    }
    
    public void ChangeFirstName(FirstName firstName)
    {
        FirstName = firstName ?? throw new ValidationException("First name is required");
    }
    
    public void ChangeLastName(LastName lastName)
    {
        LastName = lastName ?? throw new ValidationException("Last name is required");
    }
    
    public void ChangePhone(PhoneNumber phone)
    {
        Phone = phone ?? throw new ValidationException("Phone number is required");
    }
    
    public void ChangeEmail(Email email)
    {
        Email = email ?? throw new ValidationException("Email is required");
    }
}