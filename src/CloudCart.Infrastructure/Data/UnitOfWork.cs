using CloudCart.Application.Abstractions;

namespace CloudCart.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly CloudCartDbContext _context;

    public UnitOfWork(CloudCartDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
