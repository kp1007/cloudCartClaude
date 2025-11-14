using CloudCart.Application.Abstractions;

namespace CloudCart.API.Features.Cart.ClearCart;

public class ClearCartCommand : ICommand
{
    public Guid CustomerId { get; }

    public ClearCartCommand(Guid customerId)
    {
        CustomerId = customerId;
    }
}
