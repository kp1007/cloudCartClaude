using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Categories.GetCategories;

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<CategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<List<CategoryResponse>>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetTopLevelCategoriesAsync(cancellationToken);

        return Result.Success(CategoryMapper.ToResponseList(categories, includeProducts: false));
    }
}
