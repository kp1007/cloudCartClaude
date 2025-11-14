using CloudCart.API.Features.Products.CreateProduct;
using CloudCart.API.Features.Products.DeleteProduct;
using CloudCart.API.Features.Products.GetProductById;
using CloudCart.API.Features.Products.GetProducts;
using CloudCart.API.Features.Products.UpdateProduct;
using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;
using CloudCart.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CloudCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICommandHandler<CreateProductCommand, ProductResponse> _createProductHandler;
    private readonly IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>> _getProductsHandler;
    private readonly IQueryHandler<GetProductByIdQuery, ProductResponse> _getProductByIdHandler;
    private readonly ICommandHandler<UpdateProductCommand> _updateProductHandler;
    private readonly ICommandHandler<DeleteProductCommand> _deleteProductHandler;

    public ProductsController(
        ICommandHandler<CreateProductCommand, ProductResponse> createProductHandler,
        IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>> getProductsHandler,
        IQueryHandler<GetProductByIdQuery, ProductResponse> getProductByIdHandler,
        ICommandHandler<UpdateProductCommand> updateProductHandler,
        ICommandHandler<DeleteProductCommand> deleteProductHandler)
    {
        _createProductHandler = createProductHandler;
        _getProductsHandler = getProductsHandler;
        _getProductByIdHandler = getProductByIdHandler;
        _updateProductHandler = updateProductHandler;
        _deleteProductHandler = deleteProductHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request);
        var result = await _createProductHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<ProductResponse>.FailureResponse(result.Error));

        return CreatedAtAction(nameof(GetProductById), new { id = result.Value.Id }, ApiResponse<ProductResponse>.SuccessResponse(result.Value));
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(pageNumber, pageSize);
        var result = await _getProductsHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<PagedResult<ProductResponse>>.FailureResponse(result.Error));

        return Ok(ApiResponse<PagedResult<ProductResponse>>.SuccessResponse(result.Value));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _getProductByIdHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(ApiResponse<ProductResponse>.FailureResponse(result.Error));

        return Ok(ApiResponse<ProductResponse>.SuccessResponse(result.Value));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(id, request);
        var result = await _updateProductHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<object>.FailureResponse(result.Error));

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);
        var result = await _deleteProductHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<object>.FailureResponse(result.Error));

        return NoContent();
    }
}
