using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Application.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand : IRequest<Result<Guid>>
{
    public CustomerId Id { get; set; }
    public FirstName FirstName { get; }
    public LastName LastName { get; }
    public PhoneNumber Phone { get; }
    public Email Email { get; }

    public UpdateCustomerCommand(
        CustomerId id,
        FirstName firstName,
        LastName lastName,
        PhoneNumber phone,
        Email email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
    }
}