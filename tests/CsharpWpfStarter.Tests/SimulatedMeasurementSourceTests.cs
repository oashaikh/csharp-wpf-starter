using CsharpWpfStarter.Core.Models;
using CsharpWpfStarter.Core.Services;
using FluentAssertions;
using Xunit;

namespace CsharpWpfStarter.Tests;

public class SimulatedMeasurementSourceTests
{
    [Fact]
    public async Task Emits_measurements_at_configured_interval()
    {
        var src = new SimulatedMeasurementSource(new Random(42))
        {
            Interval = TimeSpan.FromMilliseconds(50),
            Min = 10,
            Max = 30,
            Unit = "°C",
        };

        var received = new List<Measurement>();
        src.MeasurementReceived += (_, m) => received.Add(m);

        await src.StartAsync();
        await Task.Delay(220);
        await src.StopAsync();

        received.Should().HaveCountGreaterOrEqualTo(3);
        received.Should().AllSatisfy(m =>
        {
            m.Value.Should().BeInRange(10, 30);
            m.Unit.Should().Be("°C");
        });
    }

    [Fact]
    public async Task Latest_reflects_most_recent_reading()
    {
        var src = new SimulatedMeasurementSource(new Random(0))
        {
            Interval = TimeSpan.FromMilliseconds(20),
        };

        await src.StartAsync();
        await Task.Delay(100);
        var snapshot = src.Latest;
        await src.StopAsync();

        snapshot.Should().NotBeNull();
    }

    [Fact]
    public async Task StopAsync_is_idempotent()
    {
        var src = new SimulatedMeasurementSource();
        await src.StopAsync();
        await src.StopAsync();      // should not throw
    }
}
