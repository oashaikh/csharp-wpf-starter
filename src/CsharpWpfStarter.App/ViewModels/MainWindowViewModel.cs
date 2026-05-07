using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsharpWpfStarter.Core.Models;
using CsharpWpfStarter.Core.Services;

namespace CsharpWpfStarter.App.ViewModels;

public sealed partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly IMeasurementSource _source;
    private const int MaxMeasurements = 100;

    public ObservableCollection<Measurement> Measurements { get; } = new();

    [ObservableProperty]
    private string _status = "Idle";

    [ObservableProperty]
    private bool _isRunning;

    public MainWindowViewModel(IMeasurementSource source)
    {
        _source = source;
        _source.MeasurementReceived += OnMeasurementReceived;
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartAsync()
    {
        Status = "Running";
        IsRunning = true;
        StartCommand.NotifyCanExecuteChanged();
        StopCommand.NotifyCanExecuteChanged();
        await _source.StartAsync();
    }

    [RelayCommand(CanExecute = nameof(CanStop))]
    private async Task StopAsync()
    {
        await _source.StopAsync();
        Status = "Stopped";
        IsRunning = false;
        StartCommand.NotifyCanExecuteChanged();
        StopCommand.NotifyCanExecuteChanged();
    }

    private bool CanStart() => !IsRunning;
    private bool CanStop()  =>  IsRunning;

    private void OnMeasurementReceived(object? sender, Measurement measurement)
    {
        // Marshal back to UI thread.
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
        {
            AppendMeasurement(measurement);
        }
        else
        {
            dispatcher.BeginInvoke(() => AppendMeasurement(measurement));
        }
    }

    private void AppendMeasurement(Measurement m)
    {
        Measurements.Insert(0, m);
        while (Measurements.Count > MaxMeasurements)
            Measurements.RemoveAt(Measurements.Count - 1);
    }

    public void Dispose()
    {
        _source.MeasurementReceived -= OnMeasurementReceived;
    }
}
