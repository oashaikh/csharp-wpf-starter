using CsharpWpfStarter.Core.Models;

namespace CsharpWpfStarter.Core.Services;

/// <summary>
/// Emits a fake reading every <see cref="Interval"/> until cancelled.
/// Useful for UI work without hardware. Replace in production with a real
/// IMeasurementSource implementation (serial port, MQTT, etc.).
/// </summary>
public sealed class SimulatedMeasurementSource : IMeasurementSource
{
    private readonly Random _random;
    private CancellationTokenSource? _cts;
    private Task? _runner;

    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(1);
    public string Unit { get; init; } = "°C";
    public double Min { get; init; } = 18.0;
    public double Max { get; init; } = 24.0;

    public Measurement? Latest { get; private set; }
    public event EventHandler<Measurement>? MeasurementReceived;

    public SimulatedMeasurementSource(Random? random = null)
    {
        _random = random ?? new Random();
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _runner = Task.Run(() => RunAsync(_cts.Token));
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _cts?.Cancel();
        if (_runner is not null)
        {
            try { await _runner.WaitAsync(cancellationToken); }
            catch (OperationCanceledException) { }
        }
    }

    private async Task RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var reading = new Measurement(
                DateTimeOffset.UtcNow,
                Min + (_random.NextDouble() * (Max - Min)),
                Unit);

            Latest = reading;
            MeasurementReceived?.Invoke(this, reading);

            try { await Task.Delay(Interval, token); }
            catch (OperationCanceledException) { break; }
        }
    }
}
