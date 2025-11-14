using CloudCart.Application.Abstractions;
using CloudCart.Application.Mappers;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Products.CreateProduct;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductResponse>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        // Check if SKU already exists
        var existingProduct = await _productRepository.GetBySkuAsync(command.Request.SKU, cancellationToken);
        if (existingProduct != null)
        {
            return Result.Failure<ProductResponse>($"Product with SKU '{command.Request.SKU}' already exists");
        }

        // Create product entity
        var product = ProductMapper.ToEntity(command.Request);

        // Add to repository
        await _productRepository.AddAsync(product, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload product with category
        var savedProduct = await _productRepository.GetByIdAsync(product.Id, cancellationToken);

        // Return response
        return Result.Success(ProductMapper.ToResponse(savedProduct!));
    }
}
