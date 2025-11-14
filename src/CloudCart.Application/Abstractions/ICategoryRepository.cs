using CloudCart.Domain.Entities;

namespace CloudCart.Application.Abstractions;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IReadOnlyList<Category>> GetTopLevelCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdWithProductsAsync(Guid id, CancellationToken cancellationToken = default);
}
