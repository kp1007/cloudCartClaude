using CloudCart.API.Features.Orders.CancelOrder;
using CloudCart.API.Features.Orders.CreateOrder;
using CloudCart.API.Features.Orders.GetOrderById;
using CloudCart.API.Features.Orders.GetOrders;
using CloudCart.API.Features.Orders.UpdateOrderStatus;
using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;
using CloudCart.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CloudCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ICommandHandler<CreateOrderCommand, OrderResponse> _createOrderHandler;
    private readonly IQueryHandler<GetOrdersQuery, List<OrderResponse>> _getOrdersHandler;
    private readonly IQueryHandler<GetOrderByIdQuery, OrderResponse> _getOrderByIdHandler;
    private readonly ICommandHandler<UpdateOrderStatusCommand> _updateOrderStatusHandler;
    private readonly ICommandHandler<CancelOrderCommand> _cancelOrderHandler;

    public OrdersController(
        ICommandHandler<CreateOrderCommand, OrderResponse> createOrderHandler,
        IQueryHandler<GetOrdersQuery, List<OrderResponse>> getOrdersHandler,
        IQueryHandler<GetOrderByIdQuery, OrderResponse> getOrderByIdHandler,
        ICommandHandler<UpdateOrderStatusCommand> updateOrderStatusHandler,
        ICommandHandler<CancelOrderCommand> cancelOrderHandler)
    {
        _createOrderHandler = createOrderHandler;
        _getOrdersHandler = getOrdersHandler;
        _getOrderByIdHandler = getOrderByIdHandler;
        _updateOrderStatusHandler = updateOrderStatusHandler;
        _cancelOrderHandler = cancelOrderHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(request);
        var result = await _createOrderHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<OrderResponse>.FailureResponse(result.Error));

        return CreatedAtAction(nameof(GetOrderById), new { id = result.Value.Id }, ApiResponse<OrderResponse>.SuccessResponse(result.Value));
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] Guid customerId, CancellationToken cancellationToken)
    {
        var query = new GetOrdersQuery(customerId);
        var result = await _getOrdersHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<List<OrderResponse>>.FailureResponse(result.Error));

        return Ok(ApiResponse<List<OrderResponse>>.SuccessResponse(result.Value));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _getOrderByIdHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(ApiResponse<OrderResponse>.FailureResponse(result.Error));

        return Ok(ApiResponse<OrderResponse>.SuccessResponse(result.Value));
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateOrderStatusCommand(id, request);
        var result = await _updateOrderStatusHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<object>.FailureResponse(result.Error));

        return NoContent();
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelOrderCommand(id);
        var result = await _cancelOrderHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<object>.FailureResponse(result.Error));

        return NoContent();
    }
}
