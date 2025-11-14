using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;
using CloudCart.Domain.Entities;

namespace CloudCart.API.Features.Cart.GetCart;

public class GetCartQueryHandler : IQueryHandler<GetCartQuery, CartResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetCartQueryHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CartResponse>> HandleAsync(GetCartQuery query, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);

        if (cart == null)
        {
            // Create a new cart for the customer
            cart = ShoppingCart.Create(query.CustomerId);
            await _cartRepository.AddAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(CartMapper.ToResponse(cart));
    }
}
