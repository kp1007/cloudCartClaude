using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;

namespace CloudCart.API.Features.Products.UpdateProduct;

public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);

        if (product == null)
        {
            return Result.Failure("Product not found");
        }

        ProductMapper.UpdateEntity(product, command.Request);

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
