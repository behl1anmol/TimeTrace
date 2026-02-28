# Existing WPF Codebase Analysis

## Project Structure

```
timetrace.ui/
├── App.xaml / App.xaml.cs          — Application entry, DI, Fluent theme, tray init
├── AssemblyInfo.cs
├── Controls/
│   ├── BusyIndicator/              — IBusyAware, BusyAwareViewModel, BusyAwareView
│   └── Lightbox/                   — ILightboxService, LightboxHost, LightboxView, LightboxResult
├── Converters/
│   ├── BooleanToGridLengthConverter.cs
│   ├── BooleanToStringConverter.cs
│   ├── BooleanToVisibilityConverter.cs
│   └── StatusToBrushConverter.cs
├── Helpers/
│   ├── ViewLocator.cs              — Attached property for ViewModel→DataContext binding
│   └── ViewModelLocator.cs         — Resolves ViewModels from App.ServiceProvider
├── Models/
│   ├── ApplicationModel.cs
│   ├── CapturedImageModel.cs
│   └── FilterCriteria.cs
├── Resources/
│   ├── AppIcon/timeTrace.ico, timeTrace.png
│   └── TrayIconResources.xaml
├── Services/
│   ├── IApplicationDataService.cs / ApplicationDataService.cs  — Mock data
│   ├── INavigationService.cs / NavigationService.cs            — ViewModel-first nav
│   └── Tray/ITrayIconService.cs / TrayIconService.cs           — Win32 tray icon
├── ViewModels/
│   ├── MainViewModel.cs
│   ├── ApplicationListViewModel.cs
│   ├── ApplicationDetailsViewModel.cs
│   ├── FilterViewModel.cs
│   ├── PresenterViewModel.cs
│   └── SettingsViewModel.cs
└── Views/
    ├── MainWindow.xaml / .xaml.cs     — Root window with sidebar + ContentControl
    ├── NavigationSidebar.xaml / .cs   — Sidebar with emoji icons
    ├── ApplicationListView.xaml / .cs — ListView of apps
    ├── ApplicationDetailsView.xaml / .cs — Two-column: FilterView + PresenterView
    ├── FilterView.xaml / .cs          — Date pickers + status checkboxes
    ├── PresenterView.xaml / .cs       — UniformGrid image placeholders
    └── SettingsView.xaml / .cs        — 3 simple settings controls
```

---

## DI Registrations (App.xaml.cs)

### Services (all Singleton)
| Interface | Implementation |
|-----------|---------------|
| `INavigationService` | `NavigationService` |
| `IApplicationDataService` | `ApplicationDataService` |
| `ITrayIconService` | `TrayIconService` |
| `ILightboxHost` | `LightboxHost` |
| `ILightboxService` | `LightboxService` |

### ViewModels
| Type | Lifetime |
|------|----------|
| `MainViewModel` | Transient |
| `ApplicationListViewModel` | Transient |
| `FilterViewModel` | Singleton |
| `PresenterViewModel` | Singleton |
| `SettingsViewModel` | Singleton |
| `ApplicationDetailsViewModel` | Singleton |

### Models (registered in DI, unusual)
| Type | Lifetime |
|------|----------|
| `ApplicationModel` | Singleton |
| `CapturedImageModel` | Singleton |

**Note**: Library's repositories and DbContext are NOT registered in the UI.

---

## View ↔ ViewModel Wiring

Two mechanisms:

1. **ViewLocator attached property** — `helpers:ViewLocator.ViewModel="MainViewModel"` on each UserControl/Window. Resolves ViewModel from `ViewModelLocator` and sets `DataContext`.

2. **Implicit DataTemplates** in `MainWindow.Resources` — when `MainViewModel.CurrentView` is set to a ViewModel instance, WPF matches the DataTemplate and instantiates the corresponding View.

| View | ViewModel | Wiring |
|------|-----------|--------|
| `MainWindow` | `MainViewModel` | ViewLocator attached property |
| `NavigationSidebar` | `MainViewModel` (shared via `DataContext="{Binding}"`) | Inherits from parent |
| `ApplicationListView` | `ApplicationListViewModel` | ViewLocator + DataTemplate |
| `ApplicationDetailsView` | `ApplicationDetailsViewModel` | ViewLocator + DataTemplate |
| `FilterView` | `FilterViewModel` | ViewLocator + explicit `DataContext` from parent |
| `PresenterView` | `PresenterViewModel` | ViewLocator + explicit `DataContext` from parent |
| `SettingsView` | `SettingsViewModel` | ViewLocator + DataTemplate |

---

## Navigation Pattern

- **ViewModel-first navigation**: `MainViewModel.CurrentView` is set to a ViewModel instance. `ContentControl` in `MainWindow` uses implicit DataTemplates to display matching View.
- `MainViewModel` holds `NavigateToApplicationsCommand` and `NavigateToSettingsCommand` that directly set `CurrentView`.
- `NavigationService.Navigated` event subscribed in `MainViewModel`'s constructor for external navigation.
- `ApplicationListViewModel.SelectApplicationCommand` casts `_navigationService` to `NavigationService` to call `NavigateToDetails()`.

---

## Existing File Contents

### MainViewModel.cs
```csharp
public partial class MainViewModel : ObservableObject
{
    private readonly ApplicationListViewModel _applicationListViewModel;
    private readonly SettingsViewModel _settingsViewModel;

    [ObservableProperty] private object? currentView;
    [ObservableProperty] private string currentViewTitle = "Applications";
    [ObservableProperty] private bool isCollapsed = false;

    public MainViewModel(ApplicationListViewModel applicationListViewModel, SettingsViewModel settingsViewModel, INavigationService navigationService)
    {
        _applicationListViewModel = applicationListViewModel;
        _settingsViewModel = settingsViewModel;
        CurrentView = applicationListViewModel;
        navigationService.Navigated += (viewModel, title) => { CurrentView = viewModel; CurrentViewTitle = title ?? string.Empty; };
    }

    [RelayCommand] private void NavigateToApplications() { CurrentView = _applicationListViewModel; CurrentViewTitle = "Applications"; }
    [RelayCommand] private void NavigateToSettings() { CurrentView = _settingsViewModel; CurrentViewTitle = "Settings"; }
    [RelayCommand] private void ToggleSidebar() { IsCollapsed = !IsCollapsed; }
}
```

### ApplicationListViewModel.cs
```csharp
public partial class ApplicationListViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationDataService _dataService;
    private readonly ApplicationDetailsViewModel _applicationDetailsViewModel;

    [ObservableProperty] private ObservableCollection<ApplicationModel> applications = new();
    [ObservableProperty] private ApplicationModel? selectedApplication;

    public ApplicationListViewModel(INavigationService navigationService, ApplicationDetailsViewModel applicationDetailsViewModel, IApplicationDataService applicationDataService)
    {
        _navigationService = navigationService; _dataService = applicationDataService;
        LoadApplications(); _applicationDetailsViewModel = applicationDetailsViewModel;
    }

    private void LoadApplications() { var apps = _dataService.GetApplications(); Applications.Clear(); foreach (var app in apps) Applications.Add(app); }

    [RelayCommand] private void SelectApplication(ApplicationModel application)
    {
        SelectedApplication = application;
        var detailsViewModel = _applicationDetailsViewModel;
        if (_navigationService is NavigationService navService) navService.NavigateToDetails(detailsViewModel);
    }

    [RelayCommand] private void RefreshApplications() { LoadApplications(); }
}
```

### ApplicationDetailsViewModel.cs
```csharp
public partial class ApplicationDetailsViewModel : ObservableObject
{
    [ObservableProperty] private FilterViewModel filterViewModel;
    [ObservableProperty] private PresenterViewModel presenterViewModel;
    [ObservableProperty] private ApplicationModel application;

    public ApplicationDetailsViewModel(ApplicationModel applicationModel, INavigationService navigationService, FilterViewModel filterViewModel, PresenterViewModel presenterViewModel)
    {
        Application = applicationModel;
        FilterViewModel = filterViewModel; PresenterViewModel = presenterViewModel;
        FilterViewModel.FilterChanged += (_, _) => PresenterViewModel.ApplyFilter(FilterViewModel.FromDate, FilterViewModel.ToDate, FilterViewModel.SelectedDate, FilterViewModel.Statuses);
    }
}
```

### FilterViewModel.cs
```csharp
public partial class FilterViewModel : ObservableObject
{
    [ObservableProperty] private DateTime? fromDate;
    [ObservableProperty] private DateTime? toDate;
    [ObservableProperty] private DateTime? selectedDate;
    [ObservableProperty] private bool showActive = true;
    [ObservableProperty] private bool showMinimized = true;
    [ObservableProperty] private bool showClosed = true;

    public IList<string> Statuses { get { var s = new List<string>(); if (ShowActive) s.Add("Active"); if (ShowMinimized) s.Add("Minimized"); if (ShowClosed) s.Add("Closed"); return s; } }
    public event EventHandler? FilterChanged;

    [RelayCommand] private void ApplyFilter() { FilterChanged?.Invoke(this, EventArgs.Empty); }
    [RelayCommand] private void ClearFilter() { FromDate = null; ToDate = null; SelectedDate = null; ShowActive = true; ShowMinimized = true; ShowClosed = true; ApplyFilter(); }
}
```

### PresenterViewModel.cs
```csharp
public partial class PresenterViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<CapturedImageModel> capturedImages = new();
    private readonly int applicationId = 1;
    private readonly IApplicationDataService dataService;
    public int ImageCount => CapturedImages.Count;

    public PresenterViewModel(IApplicationDataService applicationDataService) { dataService = applicationDataService; LoadImages(); }

    public void ApplyFilter(DateTime? from, DateTime? to, DateTime? specificDay, IList<string> statuses)
    {
        var all = dataService.GetCapturedImages(applicationId);
        var filtered = all.AsQueryable();
        if (from is not null) filtered = filtered.Where(x => x.Timestamp >= from.Value);
        if (to is not null) filtered = filtered.Where(x => x.Timestamp <= to.Value);
        if (specificDay is not null) filtered = filtered.Where(x => x.Timestamp.Date == specificDay.Value.Date);
        if (statuses != null && statuses.Count > 0) filtered = filtered.Where(x => statuses.Contains(x.ProcessStatus));
        CapturedImages.Clear(); foreach (var img in filtered) CapturedImages.Add(img);
        OnPropertyChanged(nameof(ImageCount));
    }

    private void LoadImages() { ApplyFilter(null, null, null, new[] { "Active", "Minimized", "Closed" }); }
}
```

### SettingsViewModel.cs
```csharp
public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty] private bool autoStart = false;
    [ObservableProperty] private string screenshotsPath = @"C:\AppMonitor\Screenshots";
    [ObservableProperty] private bool notifyOnNewCapture = true;
    // No commands wired for save/reset
}
```

### Models

#### ApplicationModel.cs
```csharp
public partial class ApplicationModel : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string processName = string.Empty;
    [ObservableProperty] private int processId;
    [ObservableProperty] private string status = string.Empty;
    [ObservableProperty] private DateTime lastCaptured;
    [ObservableProperty] private string iconPath = string.Empty;
    [ObservableProperty] private int captureCount;
    [ObservableProperty] private bool isRunning;
    public Dictionary<string, object> ExtensionData { get; set; } = new();
}
```

#### CapturedImageModel.cs
```csharp
public partial class CapturedImageModel : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private int applicationId;
    [ObservableProperty] private string screenshotName = string.Empty;
    [ObservableProperty] private string filePath = string.Empty;
    [ObservableProperty] private DateTime timestamp;
    [ObservableProperty] private string fileSize = string.Empty;
    [ObservableProperty] private string resolution = string.Empty;
    [ObservableProperty] private string processStatus = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
```

#### FilterCriteria.cs
```csharp
public record FilterCriteria(DateTime? FromDate, DateTime? ToDate, DateTime? SelectedDate, IList<string> Statuses);
```

### Services

#### IApplicationDataService / ApplicationDataService
- `GetApplications()` → 3 hard-coded mock apps (Chrome, VS Code, Word)
- `GetCapturedImages(int applicationId)` → 2 hard-coded mock images for Chrome
- NOT connected to any real repository

#### INavigationService / NavigationService
```csharp
public interface INavigationService { event Action<object, string?>? Navigated; void NavigateTo(object viewModel); void NavigateToDetails(object detailsViewModel); }
public class NavigationService : INavigationService { public event Action<object, string?>? Navigated; public void NavigateTo(object viewModel) { Navigated?.Invoke(viewModel, null); } public void NavigateToDetails(object detailsViewModel) { Navigated?.Invoke(detailsViewModel, "Details"); } }
```

### Converters
| Class | Logic |
|-------|-------|
| `BooleanToGridLengthConverter` | `true` → `CollapsedWidth` (90), `false` → `ExpandedWidth` (250) |
| `BooleanToStringConverter` | Splits parameter on `\|`, returns first string if true, second if false |
| `BooleanToVisibilityConverter` | bool → Visible/Collapsed. Supports inversion via ConverterParameter="True" |
| `StatusToBrushConverter` | "active"→LimeGreen, "minimized"→Goldenrod, "closed"→Gray |

### Controls

#### BusyIndicator
- `IBusyAware` interface: `IsBusy`, `BusyMessage`
- `BusyAwareViewModel` — abstract base implementing `IBusyAware`
- `BusyAwareView` — overlay with dark background, white card, TextBlock + indeterminate ProgressBar

#### Lightbox
- `ILightboxService` — `ShowConfirmationAsync`, `ShowMessageAsync`, `ShowOptionsAsync`
- `LightboxService` — creates `LightboxViewModel`, calls `ILightboxHost.ShowLightboxAsync<T>`
- `LightboxHost` — creates modal Window with blur on owner
- `LightboxViewModel` — Title, Message, buttons trigger `TaskCompletionSource<LightboxResult>`
- `LightboxResult` enum: Primary, Secondary, Cancel

### App.xaml
- Merges `PresentationFramework.Fluent` theme + `TrayIconResources.xaml`
- Declares `AppIcon` BitmapImage resource
- Fluent theme auto-adapts to Windows light/dark mode

### MainWindow.xaml.cs
- Overrides `OnClosing` → shows exit-confirmation lightbox (Exit / Minimize to Tray / Cancel)

---

## Key Infrastructure Features (DO NOT MODIFY)
1. **Tray icon** — Win32 P/Invoke based, initialized after MainWindow.Loaded
2. **Lightbox** — Exit confirmation dialog with blur overlay
3. **Busy indicator** — Overlay control for async operations
4. **ViewLocator / ViewModelLocator** — Attached property based DataContext resolution
5. **Fluent theme** — `PresentationFramework.Fluent` merged in App.xaml, auto light/dark
6. **CommunityToolkit.Mvvm** — ObservableObject, ObservableProperty, RelayCommand
7. **Microsoft.Xaml.Behaviors.Wpf** — Interaction triggers for events
8. **Microsoft.Extensions.DependencyInjection** — Full DI container
