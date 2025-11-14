using CloudCart.Application.Abstractions;
using CloudCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudCart.Infrastructure.Data.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(CloudCartDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public override async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetByIdWithItemsAsync(id, cancellationToken);
    }
}
