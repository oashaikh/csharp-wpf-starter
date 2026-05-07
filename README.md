# csharp-wpf-starter

A drop-in **WPF + .NET 8 + MVVM** desktop application template, set up
the way I'd actually want to start a new project today: dependency
injection from `Microsoft.Extensions.Hosting`, view-models built on
`CommunityToolkit.Mvvm`, domain code split into a separate project, xUnit
test project pre-wired, CI on Windows.

## What this repo does

- Three-project layout:
  - `CsharpWpfStarter.Core` - pure .NET 8 (no WPF, no Win32). All your
    domain types and services. Testable on any platform.
  - `CsharpWpfStarter.App` - the WPF UI. Composition root, views,
    view-models, converters, value-binding only. References Core.
  - `CsharpWpfStarter.Tests` - xUnit + FluentAssertions tests against
    Core only.
- Microsoft.Extensions.Hosting handles startup/shutdown and DI registration.
- CommunityToolkit.Mvvm gives you `[ObservableProperty]` and
  `[RelayCommand]` so view-models stay tiny.
- Sample features: a `SimulatedMeasurementSource` and a window that
  shows live readings — replace with your real device source.

## Project layout

- `CsharpWpfStarter.sln` - solution file, three projects.
- `Directory.Build.props` - shared MSBuild properties (TFM, nullable, etc.).
- `src/CsharpWpfStarter.Core/`
  - `Models/Measurement.cs` - immutable record.
  - `Services/IMeasurementSource.cs` - data-source abstraction.
  - `Services/SimulatedMeasurementSource.cs` - fake data for dev/testing.
- `src/CsharpWpfStarter.App/`
  - `App.xaml(.cs)` - `Host.CreateDefaultBuilder` + DI.
  - `Views/MainWindow.xaml(.cs)` - the only window.
  - `ViewModels/MainWindowViewModel.cs` - bound to MainWindow.
- `tests/CsharpWpfStarter.Tests/` - xUnit tests for Core.

## Quick start

You need .NET 8 SDK on Windows.

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/CsharpWpfStarter.App
```

In Visual Studio: open `CsharpWpfStarter.sln`, set
`CsharpWpfStarter.App` as startup, F5.

## Why split Core and App

WPF projects compile slowly, can't easily target non-Windows runners, and
drag along XAML reflection costs. Pushing your domain logic to a Core
library means:

- Tests live somewhere they can run (`net8.0` instead of `net8.0-windows`).
- The same Core library can later host a console app, a service, or
  a future MAUI/Blazor port.
- View-models depend on abstractions (`IMeasurementSource`), not on
  concrete WPF or Win32 types.

## Why CommunityToolkit.Mvvm

Stops you writing `INotifyPropertyChanged` by hand. `[ObservableProperty]`
generates the backing field + setter + change notification. `[RelayCommand]`
generates an `ICommand` from a method. About 60% less boilerplate per
view-model with no runtime cost.

## Adding a new view + view-model

1. Create `Views/SettingsWindow.xaml` and `ViewModels/SettingsViewModel.cs`.
2. Register both in `App.xaml.cs` `ConfigureServices`:
   ```csharp
   services.AddSingleton<SettingsViewModel>();
   services.AddSingleton<SettingsWindow>();
   ```
3. Inject `SettingsViewModel` into `SettingsWindow`'s constructor.
4. Resolve and `.Show()` the window from wherever you need to open it.

## Packaging for distribution

For a self-contained single-file exe:

```bash
dotnet publish src/CsharpWpfStarter.App `
  -c Release -r win-x64 `
  --self-contained `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true
```

Output ends up under `src/CsharpWpfStarter.App/bin/Release/net8.0-windows/win-x64/publish/`.
