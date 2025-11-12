using Mindly.Application.DTOs.Sessions;
using Mindly.Domain.Entities;

namespace Mindly.Application.Mappings;

public static class FocusSessionMapper
{
    public static FocusSessionDto ToDto(FocusSession session)
    {
        return new FocusSessionDto
        {
            Id = session.Id,
            UserProfileId = session.UserProfileId,
            Title = session.Title,
            Description = session.Description,
            FocusMode = session.FocusMode,
            PlannedDurationMinutes = session.PlannedDuration.TotalMinutes,
            ActualDurationMinutes = session.ActualDuration?.TotalMinutes,
            PlannedStart = session.PlannedStart,
            StartedAt = session.StartedAt,
            CompletedAt = session.CompletedAt,
            Status = session.Status,
            CancellationReason = session.CancellationReason,
            TotalBreakMinutes = session.TotalBreakMinutes,
            Breaks = session.Breaks
                .Select(b => new BreakPeriodDto
                {
                    Id = b.Id,
                    BreakType = b.BreakType,
                    DurationMinutes = b.Duration.TotalMinutes,
                    StartedAt = b.StartedAt,
                    Notes = b.Notes
                })
                .ToArray(),
            DeviceSignals = session.DeviceSignals
                .Select(signal => new DeviceSignalDto
                {
                    Id = signal.Id,
                    DeviceName = signal.DeviceName,
                    SignalType = signal.SignalType,
                    Payload = signal.Payload,
                    RecordedAt = signal.RecordedAt
                })
                .ToArray(),
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt
        };
    }
}

