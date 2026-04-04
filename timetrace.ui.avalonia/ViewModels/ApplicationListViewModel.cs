using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using timetrace.ui.avalonia.Services;

namespace timetrace.ui.avalonia.ViewModels;

public partial class ApplicationListViewModel : ViewModelBase
{
    private readonly Action<ApplicationItemViewModel>? _navigateToDetails;
    private readonly IApplicationDataService? _dataService;

    [ObservableProperty]
    private ObservableCollection<ApplicationItemViewModel> _applications = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string? _errorMessage;

    public ApplicationListViewModel() : this(null, null)
    {
    }

    public ApplicationListViewModel(Action<ApplicationItemViewModel>? navigateToDetails, IApplicationDataService? dataService)
    {
        _navigateToDetails = navigateToDetails;
        _dataService = dataService;
        
        // Load data on construction
        _ = LoadApplicationsAsync();
    }

    private async Task LoadApplicationsAsync()
    {
        if (_dataService == null)
        {
            // Fallback to mock data if no service
            LoadMockData();
            return;
        }

        try
        {
            IsLoading = true;
            ErrorMessage = null;
            
            var apps = await _dataService.GetApplicationsAsync();
            
            Applications.Clear();
            foreach (var app in apps)
            {
                app.SelectCommand = new RelayCommand(() => _navigateToDetails?.Invoke(app));
                Applications.Add(app);
            }
            
            IsEmpty = Applications.Count == 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load applications: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LoadMockData()
    {
        Applications.Add(new ApplicationItemViewModel 
        { 
            Id = 1, 
            ProcessName = "Google Chrome", 
            Icon = "🌐", 
            Status = "running",
            ScreenshotCount = 24,
            MemoryUsage = "845 MB"
        });
        Applications.Add(new ApplicationItemViewModel 
        { 
            Id = 2, 
            ProcessName = "Visual Studio Code", 
            Icon = "💻", 
            Status = "running",
            ScreenshotCount = 18,
            MemoryUsage = "512 MB"
        });
        Applications.Add(new ApplicationItemViewModel 
        { 
            Id = 3, 
            ProcessName = "Slack", 
            Icon = "💬", 
            Status = "running",
            ScreenshotCount = 12,
            MemoryUsage = "324 MB"
        });
        Applications.Add(new ApplicationItemViewModel 
        { 
            Id = 4, 
            ProcessName = "Spotify", 
            Icon = "🎵", 
            Status = "idle",
            ScreenshotCount = 8,
            MemoryUsage = "256 MB"
        });
        Applications.Add(new ApplicationItemViewModel 
        { 
            Id = 5, 
            ProcessName = "Microsoft Excel", 
            Icon = "📊", 
            Status = "running",
            ScreenshotCount = 15,
            MemoryUsage = "432 MB"
        });
        Applications.Add(new ApplicationItemViewModel 
        { 
            Id = 6, 
            ProcessName = "Microsoft Outlook", 
            Icon = "📧", 
            Status = "running",
            ScreenshotCount = 20,
            MemoryUsage = "387 MB"
        });
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadApplicationsAsync();
    }
}