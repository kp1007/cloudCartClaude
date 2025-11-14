using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Products.GetProducts;

public class GetProductsQuery : IQuery<PagedResult<ProductResponse>>
{
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetProductsQuery(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
