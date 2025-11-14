using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;

namespace CloudCart.API.Features.Orders.CreateOrder;

public class CreateOrderCommand : ICommand
{
    public CreateOrderRequest Request { get; }

    public CreateOrderCommand(CreateOrderRequest request)
    {
        Request = request;
    }
}
