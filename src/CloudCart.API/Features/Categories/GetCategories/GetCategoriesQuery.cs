using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Categories.GetCategories;

public class GetCategoriesQuery : IQuery<List<CategoryResponse>>
{
}
