using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;

namespace CloudCart.API.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommand : ICommand
{
    public Guid OrderId { get; }
    public UpdateOrderStatusRequest Request { get; }

    public UpdateOrderStatusCommand(Guid orderId, UpdateOrderStatusRequest request)
    {
        OrderId = orderId;
        Request = request;
    }
}
