using timetrace.ui.Models;

namespace timetrace.ui.Services;

/// <summary>
/// Mock implementation of <see cref="IApplicationDataService"/>.
/// TODO: Wire to IProcessRepository / IImageRepository from timetrace.library
///       to replace hard-coded data with real database queries.
/// </summary>
public class ApplicationDataService : IApplicationDataService
{
    // Base date for consistent mock data — all dates relative to "now"
    private static readonly DateTime BaseDate = new(2026, 2, 28, 10, 0, 0);

    public List<ApplicationModel> GetApplications()
    {
        return new List<ApplicationModel>
        {
            new()
            {
                Id = 1, ProcessName = "Google Chrome", ProcessId = 12345,
                Status = "Active", LastCaptured = BaseDate.AddMinutes(-2),
                IconPath = "🌐", CaptureCount = 24, IsRunning = true
            },
            new()
            {
                Id = 2, ProcessName = "Visual Studio Code", ProcessId = 23456,
                Status = "Active", LastCaptured = BaseDate.AddMinutes(-4),
                IconPath = "💻", CaptureCount = 18, IsRunning = true
            },
            new()
            {
                Id = 3, ProcessName = "Slack", ProcessId = 34567,
                Status = "Active", LastCaptured = BaseDate.AddMinutes(-10),
                IconPath = "💬", CaptureCount = 12, IsRunning = true
            },
            new()
            {
                Id = 4, ProcessName = "Spotify", ProcessId = 45678,
                Status = "Minimized", LastCaptured = BaseDate.AddHours(-1),
                IconPath = "🎵", CaptureCount = 8, IsRunning = true
            },
            new()
            {
                Id = 5, ProcessName = "Microsoft Excel", ProcessId = 56789,
                Status = "Active", LastCaptured = BaseDate.AddMinutes(-15),
                IconPath = "📊", CaptureCount = 15, IsRunning = true
            },
            new()
            {
                Id = 6, ProcessName = "Microsoft Outlook", ProcessId = 67890,
                Status = "Active", LastCaptured = BaseDate.AddMinutes(-5),
                IconPath = "📧", CaptureCount = 20, IsRunning = true
            }
        };
    }

    public List<CapturedImageModel> GetCapturedImages(int applicationId) =>
        GetCapturedImagesForProcess(applicationId);

    public List<CapturedImageModel> GetCapturedImagesForProcess(int processId)
    {
        return GenerateAllImages()
            .Where(img => img.ApplicationId == processId)
            .ToList();
    }

    /// <summary>
    /// Generates a comprehensive set of mock images across all 6 applications.
    /// ImageSource is left null — XAML uses a placeholder fallback.
    /// </summary>
    private static List<CapturedImageModel> GenerateAllImages()
    {
        var images = new List<CapturedImageModel>();
        int id = 1;

        // Chrome — 6 screenshots
        foreach (var (name, minutesAgo) in new[]
        {
            ("Dashboard_View.png", 2), ("Gmail_Inbox.png", 15), ("YouTube_Home.png", 45),
            ("Google_Maps.png", 90), ("Chrome_Settings.png", 180), ("DevTools_Network.png", 300)
        })
        {
            images.Add(CreateImage(id++, 1, name, BaseDate.AddMinutes(-minutesAgo)));
        }

        // VS Code — 5 screenshots
        foreach (var (name, minutesAgo) in new[]
        {
            ("Editor_MainView.png", 4), ("Terminal_Output.png", 30), ("Extensions_Panel.png", 60),
            ("Debug_Session.png", 120), ("Git_Changes.png", 200)
        })
        {
            images.Add(CreateImage(id++, 2, name, BaseDate.AddMinutes(-minutesAgo)));
        }

        // Slack — 4 screenshots
        foreach (var (name, minutesAgo) in new[]
        {
            ("General_Channel.png", 10), ("Direct_Message.png", 50),
            ("Thread_View.png", 110), ("Huddle_Call.png", 250)
        })
        {
            images.Add(CreateImage(id++, 3, name, BaseDate.AddMinutes(-minutesAgo)));
        }

        // Spotify — 4 screenshots
        foreach (var (name, minutesAgo) in new[]
        {
            ("Now_Playing.png", 60), ("Playlist_View.png", 150),
            ("Search_Results.png", 300), ("Library_Albums.png", 500)
        })
        {
            images.Add(CreateImage(id++, 4, name, BaseDate.AddMinutes(-minutesAgo)));
        }

        // Excel — 5 screenshots
        foreach (var (name, minutesAgo) in new[]
        {
            ("Spreadsheet_Data.png", 15), ("Chart_View.png", 80),
            ("Pivot_Table.png", 160), ("Formula_Bar.png", 320), ("Print_Preview.png", 480)
        })
        {
            images.Add(CreateImage(id++, 5, name, BaseDate.AddMinutes(-minutesAgo)));
        }

        // Outlook — 5 screenshots
        foreach (var (name, minutesAgo) in new[]
        {
            ("Email_Inbox.png", 5), ("Calendar_Week.png", 40),
            ("Compose_Email.png", 100), ("Contact_List.png", 200), ("Meeting_Request.png", 350)
        })
        {
            images.Add(CreateImage(id++, 6, name, BaseDate.AddMinutes(-minutesAgo)));
        }

        return images;
    }

    private static CapturedImageModel CreateImage(int id, int appId, string name, DateTime timestamp)
    {
        return new CapturedImageModel
        {
            Id = id,
            ApplicationId = appId,
            ScreenshotName = name,
            FilePath = $"/Captures/App{appId}/{name}",
            Timestamp = timestamp,
            FileSize = "1.2 MB",
            Resolution = "1920x1080",
            ProcessStatus = "Active",
            ImageGuid = Guid.NewGuid(),
            ImageSource = null // Placeholder — XAML provides fallback
        };
    }
}
