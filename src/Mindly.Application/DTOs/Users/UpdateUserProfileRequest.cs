using System.ComponentModel.DataAnnotations;
using Mindly.Domain.Enums;

namespace Mindly.Application.DTOs.Users;

public class UpdateUserProfileRequest
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public WorkMode WorkMode { get; set; }

    [Range(15, 720, ErrorMessage = "A meta diária deve estar entre 15 e 720 minutos.")]
    public int DailyFocusGoalMinutes { get; set; } = 120;
}

