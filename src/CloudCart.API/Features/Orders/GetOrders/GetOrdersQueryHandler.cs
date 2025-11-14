using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Orders.GetOrders;

public class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, List<OrderResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<List<OrderResponse>>> HandleAsync(GetOrdersQuery query, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);

        return Result.Success(OrderMapper.ToResponseList(orders));
    }
}
