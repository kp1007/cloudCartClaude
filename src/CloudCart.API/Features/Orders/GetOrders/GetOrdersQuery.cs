using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Orders.GetOrders;

public class GetOrdersQuery : IQuery<List<OrderResponse>>
{
    public Guid CustomerId { get; }

    public GetOrdersQuery(Guid customerId)
    {
        CustomerId = customerId;
    }
}
