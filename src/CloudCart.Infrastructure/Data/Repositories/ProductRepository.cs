using CloudCart.Application.Abstractions;
using CloudCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudCart.Infrastructure.Data.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(CloudCartDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetActiveProductsAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActiveProductsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .CountAsync(cancellationToken);
    }

    public override async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
