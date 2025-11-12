using Microsoft.EntityFrameworkCore;
using Mindly.Domain.Abstractions;
using Mindly.Domain.Entities;
using Mindly.Infrastructure.Persistence;

namespace Mindly.Infrastructure.Repositories;

public class DeviceSignalRepository : IDeviceSignalRepository
{
    private readonly MindlyDbContext _dbContext;

    public DeviceSignalRepository(MindlyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeviceSignal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DeviceSignals.FirstOrDefaultAsync(signal => signal.Id == id, cancellationToken);
    }

    public async Task AddAsync(DeviceSignal signal, CancellationToken cancellationToken = default)
    {
        await _dbContext.DeviceSignals.AddAsync(signal, cancellationToken);
    }

    public async Task<IReadOnlyCollection<DeviceSignal>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var signals = await _dbContext.DeviceSignals
            .Where(signal => signal.FocusSessionId == sessionId)
            .OrderBy(signal => signal.RecordedAt)
            .ToListAsync(cancellationToken);

        return signals;
    }
}

