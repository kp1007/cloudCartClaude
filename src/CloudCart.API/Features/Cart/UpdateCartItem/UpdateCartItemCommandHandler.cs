using CloudCart.Application.Abstractions;

namespace CloudCart.API.Features.Cart.UpdateCartItem;

public class UpdateCartItemCommandHandler : ICommandHandler<UpdateCartItemCommand>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCartItemCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateCartItemCommand command, CancellationToken cancellationToken = default)
    {
        // This is a simplified implementation
        // In a real scenario, you'd need to find the cart containing this item
        var carts = await _cartRepository.GetAllAsync(cancellationToken);
        var cart = carts.FirstOrDefault(c => c.CartItems.Any(ci => ci.Id == command.CartItemId));

        if (cart == null)
        {
            return Result.Failure("Cart item not found");
        }

        try
        {
            cart.UpdateItem(command.CartItemId, command.Request.Quantity);
            await _cartRepository.UpdateAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
