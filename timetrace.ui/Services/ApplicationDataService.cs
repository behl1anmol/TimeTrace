using timetrace.ui.Models;

namespace timetrace.ui.Services;

public class ApplicationDataService : IApplicationDataService
{
    public List<ApplicationModel> GetApplications()
    {
        return new List<ApplicationModel>
        {
            new() {
                Id = 1,
                ProcessName = "Google Chrome",
                ProcessId = 12345,
                Status = "Active",
                LastCaptured = DateTime.Now.AddMinutes(-2),
                IconPath = "🌐",
                CaptureCount = 45,
                IsRunning = true,
                ExtensionData = new Dictionary<string, object>
                {
                    { "WindowTitle", "Chrome Browser" },
                    { "MemoryUsage", "256 MB" },
                    { "CpuUsage", "12%" }
                }
            },
            new() {
                Id = 2,
                ProcessName = "Visual Studio Code",
                ProcessId = 23456,
                Status = "Active",
                LastCaptured = DateTime.Now.AddMinutes(-4),
                IconPath = "💻",
                CaptureCount = 32,
                IsRunning = true,
                ExtensionData = new Dictionary<string, object>
                {
                    { "WindowTitle", "VS Code - Editor" },
                    { "MemoryUsage", "189 MB" },
                    { "CpuUsage", "8%" }
                }
            },
            new() {
                Id = 3,
                ProcessName = "Microsoft Word",
                ProcessId = 34567,
                Status = "Minimized",
                LastCaptured = DateTime.Now.AddMinutes(-7),
                IconPath = "📝",
                CaptureCount = 28,
                IsRunning = true,
                ExtensionData = new Dictionary<string, object>
                {
                    { "WindowTitle", "Document1.docx" },
                    { "MemoryUsage", "145 MB" },
                    { "CpuUsage", "3%" }
                }
            }
        };
    }

    public List<CapturedImageModel> GetCapturedImages(int applicationId)
    {
        var baseImages = new List<CapturedImageModel>
        {
            new() {
                Id = 1,
                ApplicationId = 1,
                ScreenshotName = "chrome_capture_001.png",
                FilePath = "/Captures/Chrome/chrome_capture_001.png",
                Timestamp = DateTime.Now.AddMinutes(-2),
                FileSize = "1.2 MB",
                Resolution = "1920x1080",
                ProcessStatus = "Active",
                Metadata = new Dictionary<string, object>
                {
                    { "Quality", "High" },
                    { "CompressionType", "PNG" },
                    { "ColorDepth", "24-bit" }
                }
            },
            new() {
                Id = 2,
                ApplicationId = 1,
                ScreenshotName = "chrome_capture_002.png",
                FilePath = "/Captures/Chrome/chrome_capture_002.png",
                Timestamp = DateTime.Now.AddMinutes(-5),
                FileSize = "1.1 MB",
                Resolution = "1920x1080",
                ProcessStatus = "Active"
            }
        };

        return baseImages.Where(img => img.ApplicationId == applicationId).ToList();
    }
}
