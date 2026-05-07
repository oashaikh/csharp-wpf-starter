using System.Windows;
using CsharpWpfStarter.App.ViewModels;
using CsharpWpfStarter.App.Views;
using CsharpWpfStarter.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CsharpWpfStarter.App;

public partial class App : Application
{
    private IHost? _host;

    public IServiceProvider Services => _host?.Services
        ?? throw new InvalidOperationException("Host not started yet");

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IMeasurementSource, SimulatedMeasurementSource>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        await _host.StartAsync();

        var window = Services.GetRequiredService<MainWindow>();
        window.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            using (_host)
                await _host.StopAsync(TimeSpan.FromSeconds(2));
        }
        base.OnExit(e);
    }
}
