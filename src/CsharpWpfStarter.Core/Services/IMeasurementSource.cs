using CsharpWpfStarter.Core.Models;

namespace CsharpWpfStarter.Core.Services;

/// <summary>
/// Abstraction over wherever measurements come from (sensor, simulator, file).
/// Pure-domain layer — no UI, no Win32 — so it's testable in isolation.
/// </summary>
public interface IMeasurementSource
{
    /// <summary>Snapshot of the most recent reading, if any.</summary>
    Measurement? Latest { get; }

    /// <summary>Fires when a new measurement arrives.</summary>
    event EventHandler<Measurement>? MeasurementReceived;

    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
