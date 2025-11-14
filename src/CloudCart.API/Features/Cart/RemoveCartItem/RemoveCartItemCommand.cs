using CloudCart.Application.Abstractions;

namespace CloudCart.API.Features.Cart.RemoveCartItem;

public class RemoveCartItemCommand : ICommand
{
    public Guid CartItemId { get; }

    public RemoveCartItemCommand(Guid cartItemId)
    {
        CartItemId = cartItemId;
    }
}
