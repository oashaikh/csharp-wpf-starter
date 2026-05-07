using CsharpWpfStarter.Core.Models;
using FluentAssertions;
using Xunit;

namespace CsharpWpfStarter.Tests;

public class MeasurementTests
{
    [Fact]
    public void Now_uses_current_utc_time()
    {
        var before = DateTimeOffset.UtcNow.AddSeconds(-1);
        var m = Measurement.Now(21.5, "°C");
        var after = DateTimeOffset.UtcNow.AddSeconds(1);

        m.Timestamp.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        m.Value.Should().Be(21.5);
        m.Unit.Should().Be("°C");
    }

    [Fact]
    public void Records_compare_by_value()
    {
        var ts = DateTimeOffset.Parse("2026-01-01T00:00:00Z");
        var a = new Measurement(ts, 1.0, "V");
        var b = new Measurement(ts, 1.0, "V");

        a.Should().Be(b);
    }
}
