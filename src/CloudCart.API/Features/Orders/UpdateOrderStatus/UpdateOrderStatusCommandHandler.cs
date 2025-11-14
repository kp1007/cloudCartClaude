using CloudCart.Application.Abstractions;
using CloudCart.Domain.Enums;

namespace CloudCart.API.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateOrderStatusCommand command, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure("Order not found");
        }

        // Parse status
        if (!Enum.TryParse<OrderStatus>(command.Request.Status, true, out var newStatus))
        {
            return Result.Failure($"Invalid order status: {command.Request.Status}");
        }

        try
        {
            order.UpdateStatus(newStatus);
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
