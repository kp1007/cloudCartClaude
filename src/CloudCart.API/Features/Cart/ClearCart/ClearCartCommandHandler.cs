using CloudCart.Application.Abstractions;

namespace CloudCart.API.Features.Cart.ClearCart;

public class ClearCartCommandHandler : ICommandHandler<ClearCartCommand>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ClearCartCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ClearCartCommand command, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(command.CustomerId, cancellationToken);

        if (cart == null)
        {
            return Result.Failure("Cart not found");
        }

        cart.Clear();
        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
