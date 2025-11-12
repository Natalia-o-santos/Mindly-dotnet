using Mindly.Application.DTOs.Users;
using Mindly.Domain.Entities;

namespace Mindly.Application.Mappings;

public static class UserProfileMapper
{
    public static UserProfileDto ToDto(UserProfile user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            WorkMode = user.WorkMode,
            DailyFocusGoalMinutes = user.DailyFocusGoal.TotalMinutes,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

