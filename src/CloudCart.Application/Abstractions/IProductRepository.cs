using CloudCart.Domain.Entities;

namespace CloudCart.Application.Abstractions;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetActiveProductsAsync(int skip, int take, CancellationToken cancellationToken = default);
    Task<int> GetActiveProductsCountAsync(CancellationToken cancellationToken = default);
}
