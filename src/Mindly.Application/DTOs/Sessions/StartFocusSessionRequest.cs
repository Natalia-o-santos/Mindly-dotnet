using System.ComponentModel.DataAnnotations;

namespace Mindly.Application.DTOs.Sessions;

public class StartFocusSessionRequest
{
    [Required]
    public DateTimeOffset StartedAt { get; set; }
}

