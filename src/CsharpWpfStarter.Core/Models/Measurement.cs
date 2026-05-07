namespace CsharpWpfStarter.Core.Models;

/// <summary>
/// A single timestamped sensor reading. Replace with your domain types.
/// </summary>
public sealed record Measurement(DateTimeOffset Timestamp, double Value, string Unit)
{
    public static Measurement Now(double value, string unit)
        => new(DateTimeOffset.UtcNow, value, unit);
}
