using CloudCart.Application.Abstractions;

namespace CloudCart.API.Features.Orders.CancelOrder;

public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure("Order not found");
        }

        try
        {
            order.Cancel();
            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
