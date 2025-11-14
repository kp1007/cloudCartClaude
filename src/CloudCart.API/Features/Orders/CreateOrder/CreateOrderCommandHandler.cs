using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;
using CloudCart.Domain.Entities;

namespace CloudCart.API.Features.Orders.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderResponse>> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        // Validate that all products exist and have sufficient stock
        var orderItems = new List<OrderItem>();

        foreach (var item in command.Request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);

            if (product == null)
            {
                return Result.Failure<OrderResponse>($"Product with ID '{item.ProductId}' not found");
            }

            if (product.StockQuantity < item.Quantity)
            {
                return Result.Failure<OrderResponse>($"Insufficient stock for product '{product.Name}'");
            }

            var orderItem = OrderItem.Create(item.ProductId, item.Quantity, product.Price);
            orderItems.Add(orderItem);

            // Reduce stock
            product.UpdateStock(-item.Quantity);
        }

        // Create shipping address
        var shippingAddress = OrderMapper.ToAddressEntity(command.Request.ShippingAddress);

        // Create order
        var order = Order.Create(command.Request.CustomerId, shippingAddress, orderItems);

        // Add to repository
        await _orderRepository.AddAsync(order, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload order with items
        var savedOrder = await _orderRepository.GetByIdWithItemsAsync(order.Id, cancellationToken);

        // Return response
        return Result.Success(OrderMapper.ToResponse(savedOrder!));
    }
}
