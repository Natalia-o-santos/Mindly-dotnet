using Mindly.Domain.Entities;
using Mindly.Domain.Enums;
using Mindly.Domain.ValueObjects;

namespace Mindly.Domain.Abstractions;

public interface IFocusSessionRepository
{
    Task<FocusSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(FocusSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(FocusSession session, CancellationToken cancellationToken = default);
    Task DeleteAsync(FocusSession session, CancellationToken cancellationToken = default);
    Task<PaginatedResult<FocusSession>> SearchAsync(
        Guid? userId,
        DateTimeOffset? startFrom,
        DateTimeOffset? startTo,
        FocusMode? focusMode,
        SessionStatus? status,
        string? keyword,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}

