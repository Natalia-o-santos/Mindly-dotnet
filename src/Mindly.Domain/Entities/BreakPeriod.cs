using Mindly.Domain.Enums;
using Mindly.Domain.ValueObjects;

namespace Mindly.Domain.Entities;

public class BreakPeriod : BaseEntity
{
    private int _durationMinutes;

    public BreakPeriod(Guid focusSessionId, BreakType breakType, DateTimeOffset startedAt, Duration duration, string? notes = null)
        : base()
    {
        FocusSessionId = focusSessionId;
        BreakType = breakType;
        StartedAt = startedAt;
        _durationMinutes = duration.TotalMinutes;
        Notes = notes?.Trim();
    }

    private BreakPeriod()
    {
    }

    public Guid FocusSessionId { get; private set; }
    public BreakType BreakType { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public Duration Duration => Duration.FromMinutes(_durationMinutes);
    public string? Notes { get; private set; }
}

