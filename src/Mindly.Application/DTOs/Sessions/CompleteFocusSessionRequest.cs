using System.ComponentModel.DataAnnotations;

namespace Mindly.Application.DTOs.Sessions;

public class CompleteFocusSessionRequest
{
    [Required]
    public DateTimeOffset CompletedAt { get; set; }

    [Range(5, 720, ErrorMessage = "A duração real deve estar entre 5 e 720 minutos.")]
    public int? ActualDurationMinutes { get; set; }
}

