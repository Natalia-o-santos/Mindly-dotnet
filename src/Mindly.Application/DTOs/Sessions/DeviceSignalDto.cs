namespace Mindly.Application.DTOs.Sessions;

public class DeviceSignalDto
{
    public required Guid Id { get; init; }
    public required string DeviceName { get; init; }
    public required string SignalType { get; init; }
    public required string Payload { get; init; }
    public required DateTimeOffset RecordedAt { get; init; }
}

