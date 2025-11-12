namespace Mindly.Domain.Entities;

public class DeviceSignal : BaseEntity
{
    public DeviceSignal(Guid focusSessionId, string deviceName, string signalType, string payload, DateTimeOffset recordedAt)
        : base()
    {
        if (string.IsNullOrWhiteSpace(deviceName))
        {
            throw new ArgumentException("O nome do dispositivo deve ser informado.", nameof(deviceName));
        }

        if (string.IsNullOrWhiteSpace(signalType))
        {
            throw new ArgumentException("O tipo de sinal deve ser informado.", nameof(signalType));
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("O payload deve ser informado.", nameof(payload));
        }

        FocusSessionId = focusSessionId;
        DeviceName = deviceName.Trim();
        SignalType = signalType.Trim();
        Payload = payload.Trim();
        RecordedAt = recordedAt;
    }

    private DeviceSignal()
    {
    }

    public Guid FocusSessionId { get; private set; }
    public string DeviceName { get; private set; } = string.Empty;
    public string SignalType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset RecordedAt { get; private set; }
}

