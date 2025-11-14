using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Orders.GetOrderById;

public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderResponse>> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(query.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure<OrderResponse>("Order not found");
        }

        return Result.Success(OrderMapper.ToResponse(order));
    }
}
