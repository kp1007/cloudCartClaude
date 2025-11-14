using CloudCart.Domain.Entities;

namespace CloudCart.Application.Abstractions;

public interface ICartRepository : IRepository<ShoppingCart>
{
    Task<ShoppingCart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<ShoppingCart?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
}
