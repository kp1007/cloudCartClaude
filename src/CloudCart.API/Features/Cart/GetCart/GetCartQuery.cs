using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Cart.GetCart;

public class GetCartQuery : IQuery<CartResponse>
{
    public Guid CustomerId { get; }

    public GetCartQuery(Guid customerId)
    {
        CustomerId = customerId;
    }
}
