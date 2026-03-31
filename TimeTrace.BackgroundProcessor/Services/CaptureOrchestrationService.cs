using Microsoft.Extensions.Logging;
using timetrace.library.Models;
using timetrace.library.Repositories;
using TimeTrace.Platform.Abstractions.Screenshot;

namespace TimeTrace.BackgroundProcessor.Services;

/// <summary>
/// Result of a capture and persist operation.
/// </summary>
public sealed record CaptureAndPersistResult
{
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public Image? Image { get; init; }
    public timetrace.library.Models.Process? Process { get; init; }
    public ProcessDetail? ProcessDetail { get; init; }

    public static CaptureAndPersistResult Succeeded(Image image, timetrace.library.Models.Process process, ProcessDetail processDetail) =>
        new() { Success = true, Image = image, Process = process, ProcessDetail = processDetail };

    public static CaptureAndPersistResult Failed(string error) =>
        new() { Success = false, ErrorMessage = error };

    public static CaptureAndPersistResult Skipped(string reason) =>
        new() { Success = true, ErrorMessage = reason };
}

/// <summary>
/// Service that orchestrates screenshot capture and database persistence.
/// </summary>
public interface ICaptureOrchestrationService
{
    /// <summary>
    /// Captures a screenshot and persists it to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the operation.</returns>
    Task<CaptureAndPersistResult> CaptureAndPersistAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Cleans up old temp files if the folder exceeds the configured size limit.
    /// </summary>
    /// <param name="maxSizeMB">Maximum folder size in MB.</param>
    void CleanupTempFolder(int maxSizeMB);
}

/// <summary>
/// Implementation of ICaptureOrchestrationService.
/// Coordinates between platform capture service and database repositories.
/// </summary>
public class CaptureOrchestrationService : ICaptureOrchestrationService
{
    private readonly IScreenshotCaptureService _screenshotService;
    private readonly IActiveWindowService _activeWindowService;
    private readonly IProcessRepository _processRepo;
    private readonly IImageRepository _imageRepo;
    private readonly ICaptureConfigurationService _configService;
    private readonly ILogger<CaptureOrchestrationService> _logger;

    public CaptureOrchestrationService(
        IScreenshotCaptureService screenshotService,
        IActiveWindowService activeWindowService,
        IProcessRepository processRepo,
        IImageRepository imageRepo,
        ICaptureConfigurationService configService,
        ILogger<CaptureOrchestrationService> logger)
    {
        _screenshotService = screenshotService;
        _activeWindowService = activeWindowService;
        _processRepo = processRepo;
        _imageRepo = imageRepo;
        _configService = configService;
        _logger = logger;
    }

    public async Task<CaptureAndPersistResult> CaptureAndPersistAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Check if capture service is available
            if (!_screenshotService.IsAvailable)
            {
                _logger.LogWarning("Screenshot capture service is not available");
                return CaptureAndPersistResult.Failed("No screenshot capture tool available");
            }

            // Get settings for temp path
            var settings = _configService.GetCurrentSettings();

            // Ensure temp directory exists
            if (!Directory.Exists(settings.TempFolderPath))
            {
                Directory.CreateDirectory(settings.TempFolderPath);
                _logger.LogInformation("Created temp folder: {Path}", settings.TempFolderPath);
            }

            // Generate temp file path
            var tempFileName = $"{Guid.NewGuid()}.png";
            var tempFilePath = Path.Combine(settings.TempFolderPath, tempFileName);

            // Capture screenshot to temp location
            var captureResult = await _screenshotService.CaptureActiveWindowAsync(tempFilePath, cancellationToken);

            if (!captureResult.Success)
            {
                _logger.LogWarning("Screenshot capture failed: {Error}", captureResult.ErrorMessage);
                return CaptureAndPersistResult.Failed(captureResult.ErrorMessage ?? "Capture failed");
            }

            if (!File.Exists(tempFilePath))
            {
                return CaptureAndPersistResult.Failed("Screenshot file not created");
            }

            _logger.LogDebug("Screenshot captured to temp: {Path}", tempFilePath);

            // Get or create Process entity
            var windowInfo = captureResult.WindowInfo;
            var processName = windowInfo?.ProcessName ?? "unknown";
            var windowTitle = windowInfo?.WindowTitle ?? "Unknown Window";

            // Truncate process name if needed (max 50 chars per model constraint)
            if (processName.Length > 50)
            {
                processName = processName[..50];
            }

            var process = _processRepo.GetProcessByName(processName);
            if (process is null)
            {
                process = _processRepo.AddProcess(new timetrace.library.Models.Process
                {
                    Name = processName
                });
                _logger.LogInformation("Created new process: {ProcessName}", processName);
            }

            // Create ProcessDetail for this capture session
            // Truncate description if needed (max 255 chars per model constraint)
            var description = windowTitle;
            if (description.Length > 255)
            {
                description = description[..255];
            }

            var processDetail = _processRepo.AddProcessDetail(new ProcessDetail
            {
                ProcessId = process.ProcessId,
                Description = description
            });

            // Create Image entity
            // Note: ImageRepository.AddImage auto-generates Name and ImagePath from config
            var image = _imageRepo.AddImage(new Image
            {
                ProcessDetailId = processDetail.ProcessDetailId
            });

            // Move file from temp to final location
            if (!string.IsNullOrEmpty(image.ImagePath))
            {
                var finalDir = Path.GetDirectoryName(image.ImagePath);
                if (!string.IsNullOrEmpty(finalDir) && !Directory.Exists(finalDir))
                {
                    Directory.CreateDirectory(finalDir);
                }

                File.Move(tempFilePath, image.ImagePath, overwrite: true);
                _logger.LogDebug("Moved screenshot to final path: {Path}", image.ImagePath);
            }
            else
            {
                // If ImagePath is null/empty, keep in temp
                image.ImagePath = tempFilePath;
                _logger.LogWarning("ImagePath was empty, keeping in temp: {Path}", tempFilePath);
            }

            _logger.LogInformation("Captured and persisted: Process={Process}, Window={Window}, Image={ImageId}",
                processName, description, image.ImageId);

            return CaptureAndPersistResult.Succeeded(image, process, processDetail);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during capture and persist");
            return CaptureAndPersistResult.Failed(ex.Message);
        }
    }

    public void CleanupTempFolder(int maxSizeMB)
    {
        try
        {
            var settings = _configService.GetCurrentSettings();
            var tempPath = settings.TempFolderPath;

            if (!Directory.Exists(tempPath))
            {
                return;
            }

            var files = new DirectoryInfo(tempPath)
                .GetFiles("*.png")
                .OrderBy(f => f.CreationTimeUtc)
                .ToList();

            var totalSizeBytes = files.Sum(f => f.Length);
            var maxSizeBytes = (long)maxSizeMB * 1024 * 1024;

            if (totalSizeBytes <= maxSizeBytes)
            {
                return;
            }

            _logger.LogInformation("Temp folder size ({SizeMB}MB) exceeds limit ({MaxMB}MB), cleaning up",
                totalSizeBytes / (1024 * 1024), maxSizeMB);

            var bytesToDelete = totalSizeBytes - maxSizeBytes;
            var deletedCount = 0;

            foreach (var file in files)
            {
                if (bytesToDelete <= 0) break;

                try
                {
                    bytesToDelete -= file.Length;
                    file.Delete();
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete temp file: {Path}", file.FullName);
                }
            }

            _logger.LogInformation("Cleaned up {Count} temp files", deletedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up temp folder");
        }
    }
}
