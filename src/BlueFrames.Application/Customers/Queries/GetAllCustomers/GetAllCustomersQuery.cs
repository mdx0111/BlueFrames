using BlueFrames.Application.Customers.Queries.Common;

namespace BlueFrames.Application.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery : IRequest<Result<List<CustomerDto>>>
{
    public int Limit { get; }
    public int Offset { get; }

    public GetAllCustomersQuery(int limit, int offset)
    {
        Limit = limit;
        Offset = offset;
    }
}