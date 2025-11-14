using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;

namespace CloudCart.API.Features.Cart.UpdateCartItem;

public class UpdateCartItemCommand : ICommand
{
    public Guid CartItemId { get; }
    public UpdateCartItemRequest Request { get; }

    public UpdateCartItemCommand(Guid cartItemId, UpdateCartItemRequest request)
    {
        CartItemId = cartItemId;
        Request = request;
    }
}
