using System.ComponentModel.DataAnnotations;

namespace Mindly.Application.DTOs.Sessions;

public class CancelFocusSessionRequest
{
    [MaxLength(300)]
    public string? Reason { get; set; }
}

