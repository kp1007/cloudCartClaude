using CloudCart.Contracts.Responses;
using CloudCart.Domain.Entities;

namespace CloudCart.Application.Mappers;

public static class CartMapper
{
    public static CartResponse ToResponse(ShoppingCart cart)
    {
        return new CartResponse
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            CartItems = cart.CartItems?.Select(ToCartItemResponse).ToList() ?? new List<CartItemResponse>(),
            TotalAmount = cart.GetTotalAmount()
        };
    }

    public static CartItemResponse ToCartItemResponse(CartItem cartItem)
    {
        return new CartItemResponse
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            ProductName = cartItem.Product?.Name ?? string.Empty,
            Quantity = cartItem.Quantity,
            UnitPrice = cartItem.UnitPrice,
            TotalPrice = cartItem.GetTotalPrice()
        };
    }
}
