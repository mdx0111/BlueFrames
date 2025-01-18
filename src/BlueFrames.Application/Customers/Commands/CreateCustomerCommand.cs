namespace BlueFrames.Application.Customers.Commands;

public class CreateCustomerCommand : IRequest<Result<Guid>>
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Phone { get; }
    public string Email { get; }

    public CreateCustomerCommand(string firstName, string lastName, string phone, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
    }
}