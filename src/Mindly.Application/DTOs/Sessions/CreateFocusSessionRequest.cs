using System.ComponentModel.DataAnnotations;
using Mindly.Domain.Enums;

namespace Mindly.Application.DTOs.Sessions;

public class CreateFocusSessionRequest
{
    [Required]
    public Guid UserProfileId { get; set; }

    [Required]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public FocusMode FocusMode { get; set; }

    [Range(10, 480, ErrorMessage = "A duração planejada deve estar entre 10 e 480 minutos.")]
    public int PlannedDurationMinutes { get; set; } = 25;

    [Required]
    public DateTimeOffset PlannedStart { get; set; }
}

