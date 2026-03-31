using timetrace.library.Repositories;
using timetrace.ui.avalonia.ViewModels;

namespace timetrace.ui.avalonia.Services;

/// <summary>
/// Service for loading application and image data from the database
/// </summary>
public interface IApplicationDataService
{
    Task<List<ApplicationItemViewModel>> GetApplicationsAsync();
    Task<List<CapturedImageViewModel>> GetImagesForProcessAsync(int processId, int page = 1, int pageSize = 100);
    Task<List<CapturedImageViewModel>> GetImagesByDateRangeAsync(int processId, DateTime? from, DateTime? to);
}

public class ApplicationDataService : IApplicationDataService
{
    private readonly IProcessRepository _processRepository;
    private readonly IImageRepository _imageRepository;

    public ApplicationDataService(IProcessRepository processRepository, IImageRepository imageRepository)
    {
        _processRepository = processRepository;
        _imageRepository = imageRepository;
    }

    public async Task<List<ApplicationItemViewModel>> GetApplicationsAsync()
    {
        var processes = await Task.Run(() => _processRepository.GetProcesses(1, 100));
        
        return processes.Select(p => new ApplicationItemViewModel
        {
            Id = p.ProcessId,
            ProcessName = p.Name,
            Icon = GetIconForProcess(p.Name),
            Status = "Active",
            ScreenshotCount = p.ProcessDetails?.Sum(pd => pd.Images?.Count ?? 0) ?? 0,
            MemoryUsage = "N/A"
        }).ToList();
    }

    public async Task<List<CapturedImageViewModel>> GetImagesForProcessAsync(int processId, int page = 1, int pageSize = 100)
    {
        var processDetails = await Task.Run(() => _processRepository.GetProcessDetails(processId, page, pageSize));
        var result = new List<CapturedImageViewModel>();

        foreach (var detail in processDetails)
        {
            if (detail.Images == null) continue;
            
            foreach (var image in detail.Images)
            {
                result.Add(new CapturedImageViewModel
                {
                    Id = image.ImageId,
                    FileName = image.Name ?? $"Screenshot_{image.ImageId}.png",
                    Timestamp = image.DateTimeStamp,
                    FilePath = image.ImagePath ?? string.Empty,
                    Resolution = "1920x1080", // Would need actual image analysis
                    FileSize = "N/A"
                });
            }
        }

        return result.OrderByDescending(i => i.Timestamp).ToList();
    }

    public async Task<List<CapturedImageViewModel>> GetImagesByDateRangeAsync(int processId, DateTime? from, DateTime? to)
    {
        var startDate = from ?? DateTime.MinValue;
        var endDate = to ?? DateTime.MaxValue;
        
        // Get all process details in date range, then filter by processId
        var allProcessDetails = await Task.Run(() => 
            _processRepository.GetProcessDetailsByDateRange(startDate, endDate, 1, 1000));
        
        var processDetails = allProcessDetails.Where(pd => pd.ProcessId == processId);
        var result = new List<CapturedImageViewModel>();

        foreach (var detail in processDetails)
        {
            if (detail.Images == null) continue;
            
            foreach (var image in detail.Images)
            {
                result.Add(new CapturedImageViewModel
                {
                    Id = image.ImageId,
                    FileName = image.Name ?? $"Screenshot_{image.ImageId}.png",
                    Timestamp = image.DateTimeStamp,
                    FilePath = image.ImagePath ?? string.Empty,
                    Resolution = "1920x1080",
                    FileSize = "N/A"
                });
            }
        }

        return result.OrderByDescending(i => i.Timestamp).ToList();
    }

    private static string GetIconForProcess(string processName)
    {
        var name = processName.ToLowerInvariant();
        return name switch
        {
            var n when n.Contains("chrome") => "🌐",
            var n when n.Contains("firefox") => "🦊",
            var n when n.Contains("code") || n.Contains("vscode") => "💻",
            var n when n.Contains("slack") => "💬",
            var n when n.Contains("spotify") => "🎵",
            var n when n.Contains("terminal") || n.Contains("konsole") || n.Contains("gnome-terminal") => "🖥️",
            var n when n.Contains("nautilus") || n.Contains("files") => "📁",
            var n when n.Contains("gimp") => "🎨",
            var n when n.Contains("vlc") || n.Contains("video") => "🎬",
            _ => "📱"
        };
    }
}

/// <summary>
/// Mock implementation for design-time and testing
/// </summary>
public class MockApplicationDataService : IApplicationDataService
{
    public Task<List<ApplicationItemViewModel>> GetApplicationsAsync()
    {
        var apps = new List<ApplicationItemViewModel>
        {
            new() { Id = 1, ProcessName = "Google Chrome", Icon = "🌐", Status = "Active", ScreenshotCount = 156, MemoryUsage = "542 MB" },
            new() { Id = 2, ProcessName = "Visual Studio Code", Icon = "💻", Status = "Active", ScreenshotCount = 89, MemoryUsage = "312 MB" },
            new() { Id = 3, ProcessName = "Slack", Icon = "💬", Status = "Minimized", ScreenshotCount = 45, MemoryUsage = "198 MB" },
            new() { Id = 4, ProcessName = "Spotify", Icon = "🎵", Status = "Active", ScreenshotCount = 23, MemoryUsage = "156 MB" },
            new() { Id = 5, ProcessName = "Firefox", Icon = "🦊", Status = "Active", ScreenshotCount = 67, MemoryUsage = "423 MB" },
            new() { Id = 6, ProcessName = "Terminal", Icon = "🖥️", Status = "Active", ScreenshotCount = 34, MemoryUsage = "45 MB" }
        };
        return Task.FromResult(apps);
    }

    public Task<List<CapturedImageViewModel>> GetImagesForProcessAsync(int processId, int page = 1, int pageSize = 100)
    {
        var baseDate = DateTime.Now.AddDays(-7);
        var images = Enumerable.Range(1, 25).Select(i => new CapturedImageViewModel
        {
            Id = i,
            FileName = $"Screenshot_{i:D3}.png",
            Timestamp = baseDate.AddHours(i * 3),
            FilePath = $"/captures/process_{processId}/{i:D3}.png",
            Resolution = "1920x1080",
            FileSize = $"{1.2 + i * 0.1:F1} MB"
        }).ToList();
        
        return Task.FromResult(images);
    }

    public Task<List<CapturedImageViewModel>> GetImagesByDateRangeAsync(int processId, DateTime? from, DateTime? to)
    {
        return GetImagesForProcessAsync(processId);
    }
}
