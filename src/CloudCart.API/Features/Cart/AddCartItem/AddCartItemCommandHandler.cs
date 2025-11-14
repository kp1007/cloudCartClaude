using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;
using CloudCart.Domain.Entities;

namespace CloudCart.API.Features.Cart.AddCartItem;

public class AddCartItemCommandHandler : ICommandHandler<AddCartItemCommand, CartResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCartItemCommandHandler(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CartResponse>> HandleAsync(AddCartItemCommand command, CancellationToken cancellationToken = default)
    {
        // Get or create cart
        var cart = await _cartRepository.GetByCustomerIdAsync(command.Request.CustomerId, cancellationToken);

        if (cart == null)
        {
            cart = ShoppingCart.Create(command.Request.CustomerId);
            await _cartRepository.AddAsync(cart, cancellationToken);
        }

        // Validate product
        var product = await _productRepository.GetByIdAsync(command.Request.ProductId, cancellationToken);

        if (product == null)
        {
            return Result.Failure<CartResponse>("Product not found");
        }

        if (!product.IsActive)
        {
            return Result.Failure<CartResponse>("Product is not available");
        }

        if (product.StockQuantity < command.Request.Quantity)
        {
            return Result.Failure<CartResponse>($"Insufficient stock. Available: {product.StockQuantity}");
        }

        // Add item to cart
        cart.AddItem(command.Request.ProductId, command.Request.Quantity, product.Price);

        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload cart with items
        cart = await _cartRepository.GetByIdWithItemsAsync(cart.Id, cancellationToken);

        return Result.Success(CartMapper.ToResponse(cart!));
    }
}
