namespace CloudCart.Application.Abstractions;

/// <summary>
/// Unit of Work pattern for managing transactions
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
