using CloudCart.Application.Abstractions;
using CloudCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudCart.Infrastructure.Data.Repositories;

public class CartRepository : Repository<ShoppingCart>, ICartRepository
{
    public CartRepository(CloudCartDbContext context) : base(context)
    {
    }

    public async Task<ShoppingCart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(sc => sc.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(sc => sc.CustomerId == customerId, cancellationToken);
    }

    public async Task<ShoppingCart?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(sc => sc.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(sc => sc.Id == id, cancellationToken);
    }

    public override async Task<ShoppingCart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetByIdWithItemsAsync(id, cancellationToken);
    }
}
