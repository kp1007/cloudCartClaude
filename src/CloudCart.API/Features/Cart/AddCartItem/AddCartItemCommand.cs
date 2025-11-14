using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;

namespace CloudCart.API.Features.Cart.AddCartItem;

public class AddCartItemCommand : ICommand
{
    public AddCartItemRequest Request { get; }

    public AddCartItemCommand(AddCartItemRequest request)
    {
        Request = request;
    }
}
