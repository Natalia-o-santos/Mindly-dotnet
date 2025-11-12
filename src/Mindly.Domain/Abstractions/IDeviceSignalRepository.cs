using Mindly.Domain.Entities;

namespace Mindly.Domain.Abstractions;

public interface IDeviceSignalRepository
{
    Task<DeviceSignal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(DeviceSignal signal, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DeviceSignal>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
}

