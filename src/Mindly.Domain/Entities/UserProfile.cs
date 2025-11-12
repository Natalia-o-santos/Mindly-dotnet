using Mindly.Domain.Enums;
using Mindly.Domain.ValueObjects;

namespace Mindly.Domain.Entities;

public class UserProfile : BaseEntity
{
    private readonly List<FocusSession> _sessions = new();

    private int _dailyFocusGoalMinutes;

    public UserProfile(string name, string email, WorkMode workMode, Duration dailyFocusGoal)
    {
        SetName(name);
        SetEmail(email);
        WorkMode = workMode;
        _dailyFocusGoalMinutes = dailyFocusGoal.TotalMinutes;
    }

    private UserProfile()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public WorkMode WorkMode { get; private set; }
    public Duration DailyFocusGoal => Duration.FromMinutes(_dailyFocusGoalMinutes);
    public IReadOnlyCollection<FocusSession> Sessions => _sessions.AsReadOnly();

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome deve ser informado.", nameof(name));
        }

        Name = name.Trim();
        Touch();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O e-mail deve ser informado.", nameof(email));
        }

        if (!email.Contains('@'))
        {
            throw new ArgumentException("Formato de e-mail inválido.", nameof(email));
        }

        Email = email.Trim().ToLowerInvariant();
        Touch();
    }

    public void ChangeWorkMode(WorkMode workMode)
    {
        WorkMode = workMode;
        Touch();
    }

    public void UpdateDailyFocusGoal(Duration goal)
    {
        _dailyFocusGoalMinutes = goal.TotalMinutes;
        Touch();
    }

    public FocusSession ScheduleSession(
        string title,
        string? description,
        FocusMode mode,
        Duration plannedDuration,
        DateTimeOffset plannedStart)
    {
        var session = new FocusSession(Id, title, description, mode, plannedDuration, plannedStart);
        _sessions.Add(session);
        return session;
    }
}

