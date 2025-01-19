using BlueFrames.Application.Customers.Queries.Common;
using BlueFrames.Domain.Customers.Common;

namespace BlueFrames.Application.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery : IRequest<Result<CustomerDto>>
{
    public CustomerId Id { get; }

    public GetCustomerByIdQuery(CustomerId id)
    {
        Id = id;
    }
}