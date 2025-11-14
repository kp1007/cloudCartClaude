using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Products.GetProductById;

public class GetProductByIdQuery : IQuery<ProductResponse>
{
    public Guid ProductId { get; }

    public GetProductByIdQuery(Guid productId)
    {
        ProductId = productId;
    }
}
