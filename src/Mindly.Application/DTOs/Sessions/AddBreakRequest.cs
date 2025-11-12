using System.ComponentModel.DataAnnotations;
using Mindly.Domain.Enums;

namespace Mindly.Application.DTOs.Sessions;

public class AddBreakRequest
{
    [Required]
    public BreakType BreakType { get; set; }

    [Range(3, 60, ErrorMessage = "A duração da pausa deve estar entre 3 e 60 minutos.")]
    public int DurationMinutes { get; set; } = 5;

    [Required]
    public DateTimeOffset StartedAt { get; set; }

    [MaxLength(200)]
    public string? Notes { get; set; }
}

