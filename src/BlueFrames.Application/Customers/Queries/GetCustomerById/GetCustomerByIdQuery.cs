using BlueFrames.Application.Customers.Queries.Common;

namespace BlueFrames.Application.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery : IRequest<Result<CustomerDto>>
{
    public Guid Id { get; }

    public GetCustomerByIdQuery(Guid id)
    {
        Id = id;
    }
}