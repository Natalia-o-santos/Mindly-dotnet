using Microsoft.EntityFrameworkCore;
using Mindly.Domain.Abstractions;
using Mindly.Domain.Entities;
using Mindly.Domain.Enums;
using Mindly.Domain.ValueObjects;
using Mindly.Infrastructure.Persistence;

namespace Mindly.Infrastructure.Repositories;

public class FocusSessionRepository : IFocusSessionRepository
{
    private readonly MindlyDbContext _dbContext;

    public FocusSessionRepository(MindlyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FocusSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FocusSessions
            .Include(session => session.Breaks)
            .Include(session => session.DeviceSignals)
            .FirstOrDefaultAsync(session => session.Id == id, cancellationToken);
    }

    public async Task AddAsync(FocusSession session, CancellationToken cancellationToken = default)
    {
        await _dbContext.FocusSessions.AddAsync(session, cancellationToken);
    }

    public Task UpdateAsync(FocusSession session, CancellationToken cancellationToken = default)
    {
        _dbContext.FocusSessions.Update(session);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(FocusSession session, CancellationToken cancellationToken = default)
    {
        _dbContext.FocusSessions.Remove(session);
        return Task.CompletedTask;
    }

    public async Task<PaginatedResult<FocusSession>> SearchAsync(
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
        CancellationToken cancellationToken = default)
    {
        IQueryable<FocusSession> query = _dbContext.FocusSessions
            .Include(session => session.Breaks)
            .Include(session => session.DeviceSignals)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(session => session.UserProfileId == userId.Value);
        }

        if (startFrom.HasValue)
        {
            query = query.Where(session => session.PlannedStart >= startFrom.Value);
        }

        if (startTo.HasValue)
        {
            query = query.Where(session => session.PlannedStart <= startTo.Value);
        }

        if (focusMode.HasValue)
        {
            query = query.Where(session => session.FocusMode == focusMode.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(session => session.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(session =>
                EF.Functions.Like(session.Title, $"%{keyword}%") ||
                (session.Description != null && EF.Functions.Like(session.Description, $"%{keyword}%")));
        }

        query = ApplySorting(query, sortBy, descending);

        var totalCount = await query.CountAsync(cancellationToken);
        var sessions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<FocusSession>(sessions, totalCount, pageNumber, pageSize);
    }

    private static IQueryable<FocusSession> ApplySorting(IQueryable<FocusSession> query, string? sortBy, bool descending)
    {
        return (sortBy?.ToLowerInvariant()) switch
        {
            "title" => descending
                ? query.OrderByDescending(session => session.Title)
                : query.OrderBy(session => session.Title),
            "status" => descending
                ? query.OrderByDescending(session => session.Status)
                : query.OrderBy(session => session.Status),
            "createdat" => descending
                ? query.OrderByDescending(session => session.CreatedAt)
                : query.OrderBy(session => session.CreatedAt),
            "plannedstart" or _ => descending
                ? query.OrderByDescending(session => session.PlannedStart)
                : query.OrderBy(session => session.PlannedStart)
        };
    }
}

