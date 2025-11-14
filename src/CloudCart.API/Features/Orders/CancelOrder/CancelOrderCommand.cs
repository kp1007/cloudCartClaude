using CloudCart.Application.Abstractions;

namespace CloudCart.API.Features.Orders.CancelOrder;

public class CancelOrderCommand : ICommand
{
    public Guid OrderId { get; }

    public CancelOrderCommand(Guid orderId)
    {
        OrderId = orderId;
    }
}
