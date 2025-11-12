using Mindly.Domain.Enums;

namespace Mindly.Application.DTOs.Sessions;

public class BreakPeriodDto
{
    public required Guid Id { get; init; }
    public required BreakType BreakType { get; init; }
    public required int DurationMinutes { get; init; }
    public required DateTimeOffset StartedAt { get; init; }
    public string? Notes { get; init; }
}

