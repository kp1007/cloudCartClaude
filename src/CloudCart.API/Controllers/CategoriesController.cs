using CloudCart.API.Features.Categories.GetCategories;
using CloudCart.API.Features.Categories.GetCategoryById;
using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CloudCart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IQueryHandler<GetCategoriesQuery, List<CategoryResponse>> _getCategoriesHandler;
    private readonly IQueryHandler<GetCategoryByIdQuery, CategoryResponse> _getCategoryByIdHandler;

    public CategoriesController(
        IQueryHandler<GetCategoriesQuery, List<CategoryResponse>> getCategoriesHandler,
        IQueryHandler<GetCategoryByIdQuery, CategoryResponse> getCategoryByIdHandler)
    {
        _getCategoriesHandler = getCategoriesHandler;
        _getCategoryByIdHandler = getCategoryByIdHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();
        var result = await _getCategoriesHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
            return BadRequest(ApiResponse<List<CategoryResponse>>.FailureResponse(result.Error));

        return Ok(ApiResponse<List<CategoryResponse>>.SuccessResponse(result.Value));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await _getCategoryByIdHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
            return NotFound(ApiResponse<CategoryResponse>.FailureResponse(result.Error));

        return Ok(ApiResponse<CategoryResponse>.SuccessResponse(result.Value));
    }
}
