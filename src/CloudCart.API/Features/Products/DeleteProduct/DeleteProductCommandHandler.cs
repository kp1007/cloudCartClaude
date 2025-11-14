using CloudCart.Application.Abstractions;

namespace CloudCart.API.Features.Products.DeleteProduct;

public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);

        if (product == null)
        {
            return Result.Failure("Product not found");
        }

        await _productRepository.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
