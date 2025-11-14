using CloudCart.Application.Abstractions;
using CloudCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudCart.Infrastructure.Data.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(CloudCartDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetTopLevelCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .Where(c => c.ParentCategoryId == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdWithProductsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Products.Where(p => p.IsActive))
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public override async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
