using Mindly.Domain.Enums;
using Mindly.Domain.ValueObjects;

namespace Mindly.Domain.Entities;

public class FocusSession : BaseEntity
{
    private readonly List<BreakPeriod> _breaks = new();
    private readonly List<DeviceSignal> _deviceSignals = new();
    private int _plannedDurationMinutes;
    private int? _actualDurationMinutes;

    public FocusSession(
        Guid userProfileId,
        string title,
        string? description,
        FocusMode focusMode,
        Duration plannedDuration,
        DateTimeOffset plannedStart)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("O título da sessão deve ser informado.", nameof(title));
        }

        UserProfileId = userProfileId;
        Title = title.Trim();
        Description = description?.Trim();
        FocusMode = focusMode;
        _plannedDurationMinutes = plannedDuration.TotalMinutes;
        PlannedStart = plannedStart;
        Status = SessionStatus.Planned;
    }

    private FocusSession()
    {
    }

    public Guid UserProfileId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public FocusMode FocusMode { get; private set; }
    public Duration PlannedDuration => Duration.FromMinutes(_plannedDurationMinutes);
    public Duration? ActualDuration => _actualDurationMinutes.HasValue ? Duration.FromMinutes(_actualDurationMinutes.Value) : null;
    public DateTimeOffset PlannedStart { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public SessionStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public IReadOnlyCollection<BreakPeriod> Breaks => _breaks.AsReadOnly();
    public IReadOnlyCollection<DeviceSignal> DeviceSignals => _deviceSignals.AsReadOnly();

    public int TotalBreakMinutes => _breaks.Sum(b => b.Duration.TotalMinutes);

    public void Start(DateTimeOffset startAt)
    {
        if (Status != SessionStatus.Planned)
        {
            throw new InvalidOperationException("A sessão só pode ser iniciada quando está planejada.");
        }

        StartedAt = startAt;
        Status = SessionStatus.InProgress;
        Touch();
    }

    public void Complete(DateTimeOffset completedAt, Duration? actualDuration = null)
    {
        if (Status is not SessionStatus.Planned and not SessionStatus.InProgress)
        {
            throw new InvalidOperationException("A sessão só pode ser finalizada se estiver planejada ou em andamento.");
        }

        if (StartedAt.HasValue && completedAt < StartedAt.Value)
        {
            throw new InvalidOperationException("A data de conclusão não pode ser anterior ao início.");
        }

        CompletedAt = completedAt;
        var finalDuration = actualDuration ?? CalculateActualDuration(completedAt);
        _actualDurationMinutes = finalDuration.TotalMinutes;
        Status = SessionStatus.Completed;
        Touch();
    }

    public void Cancel(string reason)
    {
        if (Status == SessionStatus.Completed)
        {
            throw new InvalidOperationException("Não é possível cancelar uma sessão concluída.");
        }

        Status = SessionStatus.Cancelled;
        CancellationReason = string.IsNullOrWhiteSpace(reason) ? "Sem justificativa informada." : reason.Trim();
        Touch();
    }

    public BreakPeriod AddBreak(BreakType breakType, Duration duration, DateTimeOffset startedAt, string? notes = null)
    {
        if (Status == SessionStatus.Completed || Status == SessionStatus.Cancelled)
        {
            throw new InvalidOperationException("Não é possível adicionar pausas após o término da sessão.");
        }

        var projectedBreakMinutes = TotalBreakMinutes + duration.TotalMinutes;
        if (projectedBreakMinutes > PlannedDuration.TotalMinutes)
        {
            throw new InvalidOperationException("O tempo total de pausas não pode exceder a duração planejada da sessão.");
        }

        var breakPeriod = new BreakPeriod(Id, breakType, startedAt, duration, notes);
        _breaks.Add(breakPeriod);
        Touch();
        return breakPeriod;
    }

    public DeviceSignal RecordDeviceSignal(string deviceName, string signalType, string payload, DateTimeOffset recordedAt)
    {
        if (Status == SessionStatus.Cancelled)
        {
            throw new InvalidOperationException("Não é possível registrar sinais para sessões canceladas.");
        }

        var signal = new DeviceSignal(Id, deviceName, signalType, payload, recordedAt);
        _deviceSignals.Add(signal);
        Touch();
        return signal;
    }

    private Duration CalculateActualDuration(DateTimeOffset completedAt)
    {
        if (StartedAt.HasValue)
        {
            var totalMinutes = (int)Math.Max(1, Math.Ceiling((completedAt - StartedAt.Value).TotalMinutes));
            return Duration.FromMinutes(totalMinutes);
        }

        return PlannedDuration;
    }
}

