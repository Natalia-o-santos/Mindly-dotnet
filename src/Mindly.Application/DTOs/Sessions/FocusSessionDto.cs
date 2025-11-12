using Mindly.Domain.Enums;

namespace Mindly.Application.DTOs.Sessions;

public class FocusSessionDto
{
    public required Guid Id { get; init; }
    public required Guid UserProfileId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required FocusMode FocusMode { get; init; }
    public required int PlannedDurationMinutes { get; init; }
    public int? ActualDurationMinutes { get; init; }
    public required DateTimeOffset PlannedStart { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public required SessionStatus Status { get; init; }
    public string? CancellationReason { get; init; }
    public required int TotalBreakMinutes { get; init; }
    public required IReadOnlyCollection<BreakPeriodDto> Breaks { get; init; }
    public required IReadOnlyCollection<DeviceSignalDto> DeviceSignals { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

