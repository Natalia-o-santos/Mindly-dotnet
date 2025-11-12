using Mindly.Domain.Abstractions;

namespace Mindly.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly MindlyDbContext _dbContext;

    public UnitOfWork(MindlyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

