using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Categories.GetCategoryById;

public class GetCategoryByIdQuery : IQuery<CategoryResponse>
{
    public Guid CategoryId { get; }

    public GetCategoryByIdQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }
}
