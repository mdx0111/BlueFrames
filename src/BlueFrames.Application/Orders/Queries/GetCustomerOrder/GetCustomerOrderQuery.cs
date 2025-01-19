using BlueFrames.Application.Orders.Queries.Common;

namespace BlueFrames.Application.Orders.Queries.GetCustomerOrder;

public record GetCustomerOrderQuery : IRequest<Result<OrderDto>>
{
    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    
    public GetCustomerOrderQuery(Guid orderId, Guid customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
    }
}