using Mindly.Domain.Enums;

namespace Mindly.Application.DTOs.Users;

public class UserProfileDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required WorkMode WorkMode { get; init; }
    public required int DailyFocusGoalMinutes { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

