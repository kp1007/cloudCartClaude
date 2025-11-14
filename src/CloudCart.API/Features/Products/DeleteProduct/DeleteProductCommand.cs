using CloudCart.Application.Abstractions;

namespace CloudCart.API.Features.Products.DeleteProduct;

public class DeleteProductCommand : ICommand
{
    public Guid ProductId { get; }

    public DeleteProductCommand(Guid productId)
    {
        ProductId = productId;
    }
}
