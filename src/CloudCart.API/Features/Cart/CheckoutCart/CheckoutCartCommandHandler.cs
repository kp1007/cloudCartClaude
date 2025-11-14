using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;
using CloudCart.Domain.Entities;

namespace CloudCart.API.Features.Cart.CheckoutCart;

public class CheckoutCartCommandHandler : ICommandHandler<CheckoutCartCommand, OrderResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutCartCommandHandler(
        ICartRepository cartRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderResponse>> HandleAsync(CheckoutCartCommand command, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(command.CustomerId, cancellationToken);

        if (cart == null || !cart.CartItems.Any())
        {
            return Result.Failure<OrderResponse>("Cart is empty");
        }

        // Create order items from cart items
        var orderItems = new List<OrderItem>();

        foreach (var cartItem in cart.CartItems)
        {
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId, cancellationToken);

            if (product == null)
            {
                return Result.Failure<OrderResponse>($"Product not found: {cartItem.ProductId}");
            }

            if (product.StockQuantity < cartItem.Quantity)
            {
                return Result.Failure<OrderResponse>($"Insufficient stock for product '{product.Name}'");
            }

            var orderItem = OrderItem.Create(cartItem.ProductId, cartItem.Quantity, product.Price);
            orderItems.Add(orderItem);

            // Reduce stock
            product.UpdateStock(-cartItem.Quantity);
        }

        // Create shipping address
        var shippingAddress = OrderMapper.ToAddressEntity(command.ShippingAddress);

        // Create order
        var order = Order.Create(command.CustomerId, shippingAddress, orderItems);

        // Add order
        await _orderRepository.AddAsync(order, cancellationToken);

        // Clear cart
        cart.Clear();
        await _cartRepository.UpdateAsync(cart, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload order with items
        var savedOrder = await _orderRepository.GetByIdWithItemsAsync(order.Id, cancellationToken);

        return Result.Success(OrderMapper.ToResponse(savedOrder!));
    }
}
