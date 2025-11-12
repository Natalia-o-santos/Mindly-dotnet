namespace Mindly.Domain.ValueObjects;

public readonly struct Duration
{
    public int TotalMinutes { get; }

    private Duration(int totalMinutes)
    {
        if (totalMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalMinutes), "A duração deve ser maior que zero minutos.");
        }

        TotalMinutes = totalMinutes;
    }

    public static Duration FromMinutes(int minutes) => new(minutes);

    public static Duration FromTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeSpan), "A duração deve ser maior que zero minutos.");
        }

        return new Duration((int)Math.Ceiling(timeSpan.TotalMinutes));
    }

    public TimeSpan ToTimeSpan() => TimeSpan.FromMinutes(TotalMinutes);

    public override string ToString() => $"{TotalMinutes} minutos";
}

