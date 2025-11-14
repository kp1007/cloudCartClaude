using CloudCart.API.Features.Cart.AddCartItem;
using CloudCart.API.Features.Cart.CheckoutCart;
using CloudCart.API.Features.Cart.ClearCart;
using CloudCart.API.Features.Cart.GetCart;
using CloudCart.API.Features.Cart.RemoveCartItem;
using CloudCart.API.Features.Cart.UpdateCartItem;
using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;
using CloudCart.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CloudCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IQueryHandler<GetCartQuery, CartResponse> _getCartHandler;
    private readonly ICommandHandler<AddCartItemCommand, CartResponse> _addCartItemHandler;
    private readonly ICommandHandler<UpdateCartItemCommand> _updateCartItemHandler;
    private readonly ICommandHandler<RemoveCartItemCommand> _removeCartItemHandler;
    private readonly ICommandHandler<ClearCartCommand> _clearCartHandler;
    private readonly ICommandHandler<CheckoutCartCommand, OrderResponse> _checkoutCartHandler;

    public CartController(
        IQueryHandler<GetCartQuery, CartResponse> getCartHandler,
        ICommandHandler<AddCartItemCommand, CartResponse> addCartItemHandler,
        ICommandHandler<UpdateCartItemCommand> updateCartItemHandler,
        ICommandHandler<RemoveCartItemCommand> removeCartItemHandler,
        ICommandHandler<ClearCartCommand> clearCartHandler,
        ICommandHandler<CheckoutCartCommand, OrderResponse> checkoutCartHandler)
    {
        _getCartHandler = getCartHandler;
        _addCartItemHandler = addCartItemHandler;
        _updateCartItemHandler = updateCartItemHandler;
        _removeCartItemHandler = removeCartItemHandler;
        _clearCartHandler = clearCartHandler;
        _checkoutCartHandler = checkoutCartHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart([FromQuery] Guid customerId, CancellationToken cancellationToken)
    {
        var query = new GetCartQuery(customerId);
        var result = await _getCartHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<CartResponse>.FailureResponse(result.Error));

        return Ok(ApiResponse<CartResponse>.SuccessResponse(result.Value));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddCartItem([FromBody] AddCartItemRequest request, CancellationToken cancellationToken)
    {
        var command = new AddCartItemCommand(request);
        var result = await _addCartItemHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<CartResponse>.FailureResponse(result.Error));

        return Ok(ApiResponse<CartResponse>.SuccessResponse(result.Value));
    }

    [HttpPut("items/{itemId}")]
    public async Task<IActionResult> UpdateCartItem(Guid itemId, [FromBody] UpdateCartItemRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCartItemCommand(itemId, request);
        var result = await _updateCartItemHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<object>.FailureResponse(result.Error));

        return NoContent();
    }

    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveCartItem(Guid itemId, CancellationToken cancellationToken)
    {
        var command = new RemoveCartItemCommand(itemId);
        var result = await _removeCartItemHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<object>.FailureResponse(result.Error));

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart([FromQuery] Guid customerId, CancellationToken cancellationToken)
    {
        var command = new ClearCartCommand(customerId);
        var result = await _clearCartHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<object>.FailureResponse(result.Error));

        return NoContent();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckoutCart([FromQuery] Guid customerId, [FromBody] AddressRequest shippingAddress, CancellationToken cancellationToken)
    {
        var command = new CheckoutCartCommand(customerId, shippingAddress);
        var result = await _checkoutCartHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<OrderResponse>.FailureResponse(result.Error));

        return Ok(ApiResponse<OrderResponse>.SuccessResponse(result.Value));
    }
}
