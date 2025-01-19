using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Application.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand : IRequest<Result<Guid>>
{
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public PhoneNumber Phone { get; }
    public Email Email { get; }

    public CreateCustomerCommand(
        FirstName firstName,
        LastName lastName,
        PhoneNumber phone,
        Email email)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
    }
}