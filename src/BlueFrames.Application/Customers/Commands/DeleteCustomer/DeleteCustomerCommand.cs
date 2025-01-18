namespace BlueFrames.Application.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    public DeleteCustomerCommand(Guid id)
    {
        Id = id;
    }
}