using CloudCart.Contracts.Responses;
using CloudCart.Domain.Entities;

namespace CloudCart.Application.Mappers;

public static class CategoryMapper
{
    public static CategoryResponse ToResponse(Category category, bool includeProducts = false)
    {
        var response = new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            SubCategories = category.SubCategories?.Select(sc => ToResponse(sc, false)).ToList() ?? new List<CategoryResponse>()
        };

        if (includeProducts && category.Products != null)
        {
            response.Products = ProductMapper.ToResponseList(category.Products);
        }

        return response;
    }

    public static List<CategoryResponse> ToResponseList(IEnumerable<Category> categories, bool includeProducts = false)
    {
        return categories.Select(c => ToResponse(c, includeProducts)).ToList();
    }
}
