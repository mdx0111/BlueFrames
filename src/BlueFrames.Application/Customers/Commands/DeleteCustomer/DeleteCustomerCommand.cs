using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Application.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand : IRequest<Result<Guid>>
{
    public CustomerId Id { get; }

    public DeleteCustomerCommand(CustomerId id)
    {
        Id = id;
    }
}