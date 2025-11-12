using System.ComponentModel.DataAnnotations;

namespace Mindly.Application.DTOs.Sessions;

public class RecordDeviceSignalRequest
{
    [Required]
    [MaxLength(80)]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string SignalType { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Payload { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset RecordedAt { get; set; }
}

