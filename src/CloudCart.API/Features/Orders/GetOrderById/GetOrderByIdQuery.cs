using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Orders.GetOrderById;

public class GetOrderByIdQuery : IQuery<OrderResponse>
{
    public Guid OrderId { get; }

    public GetOrderByIdQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}
