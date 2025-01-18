using BlueFrames.Domain.Customers.Common;
using BlueFrames.Domain.Orders;

namespace BlueFrames.Domain.Customers;

public class Customer : Entity, IAggregateRoot
{
    public CustomerId Id { get; private set; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public Email Email { get; private set; }

    private readonly List<Order> _orders = [];
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

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

    public void PlaceOrder(Order order)
    {
        if (order is null)
        {
            throw new ValidationException("Order is required");
        }
        
        if (order.CustomerId != Id)
        {
            throw new ValidationException("Order does not belong to this customer");
        }
        
        _orders.Add(order);
    }
}