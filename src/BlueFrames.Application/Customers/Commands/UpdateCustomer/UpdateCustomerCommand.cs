namespace BlueFrames.Application.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; set; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Phone { get; }
    public string Email { get; }

    public UpdateCustomerCommand(
        Guid id,
        string firstName,
        string lastName,
        string phone,
        string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
    }
}