using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Categories.GetCategoryById;

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<CategoryResponse>> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdWithProductsAsync(query.CategoryId, cancellationToken);

        if (category == null)
        {
            return Result.Failure<CategoryResponse>("Category not found");
        }

        return Result.Success(CategoryMapper.ToResponse(category, includeProducts: true));
    }
}
