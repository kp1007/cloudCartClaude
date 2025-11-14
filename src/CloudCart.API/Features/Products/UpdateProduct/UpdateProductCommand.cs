using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;

namespace CloudCart.API.Features.Products.UpdateProduct;

public class UpdateProductCommand : ICommand
{
    public Guid ProductId { get; }
    public UpdateProductRequest Request { get; }

    public UpdateProductCommand(Guid productId, UpdateProductRequest request)
    {
        ProductId = productId;
        Request = request;
    }
}
