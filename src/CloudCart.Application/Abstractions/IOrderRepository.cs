using CloudCart.Domain.Entities;

namespace CloudCart.Application.Abstractions;

public interface IOrderRepository : IRepository<Order>
{
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
}
