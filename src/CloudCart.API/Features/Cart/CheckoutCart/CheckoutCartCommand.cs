using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;

namespace CloudCart.API.Features.Cart.CheckoutCart;

public class CheckoutCartCommand : ICommand
{
    public Guid CustomerId { get; }
    public AddressRequest ShippingAddress { get; }

    public CheckoutCartCommand(Guid customerId, AddressRequest shippingAddress)
    {
        CustomerId = customerId;
        ShippingAddress = shippingAddress;
    }
}
