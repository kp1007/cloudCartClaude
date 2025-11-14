using CloudCart.Application.Abstractions;

namespace CloudCart.API.Features.Cart.RemoveCartItem;

public class RemoveCartItemCommandHandler : ICommandHandler<RemoveCartItemCommand>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveCartItemCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RemoveCartItemCommand command, CancellationToken cancellationToken = default)
    {
        var carts = await _cartRepository.GetAllAsync(cancellationToken);
        var cart = carts.FirstOrDefault(c => c.CartItems.Any(ci => ci.Id == command.CartItemId));

        if (cart == null)
        {
            return Result.Failure("Cart item not found");
        }

        cart.RemoveItem(command.CartItemId);
        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
