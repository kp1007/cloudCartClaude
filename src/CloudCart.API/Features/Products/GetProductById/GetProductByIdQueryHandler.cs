using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Products.GetProductById;

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductResponse>> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);

        if (product == null)
        {
            return Result.Failure<ProductResponse>("Product not found");
        }

        return Result.Success(ProductMapper.ToResponse(product));
    }
}
