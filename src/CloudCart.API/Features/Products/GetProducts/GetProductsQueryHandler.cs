using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Products.GetProducts;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<PagedResult<ProductResponse>>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken = default)
    {
        var skip = (query.PageNumber - 1) * query.PageSize;

        var products = await _productRepository.GetActiveProductsAsync(skip, query.PageSize, cancellationToken);
        var totalCount = await _productRepository.GetActiveProductsCountAsync(cancellationToken);

        var result = new PagedResult<ProductResponse>
        {
            Items = ProductMapper.ToResponseList(products),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };

        return Result.Success(result);
    }
}
