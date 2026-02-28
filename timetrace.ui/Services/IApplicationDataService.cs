using timetrace.ui.Models;

/// <summary>
/// Abstraction for application and image data access.
/// TODO: Replace with IProcessRepository + IImageRepository calls from timetrace.library.
///       GetApplications() → IProcessRepository.GetProcesses(page, pageSize)
///       GetCapturedImagesForProcess() → IProcessRepository.GetImagesForProcess(processId, page, pageSize)
/// </summary>
public interface IApplicationDataService
{
    List<ApplicationModel> GetApplications();

    /// <summary>
    /// Get captured images for a specific application by its ID.
    /// </summary>
    List<CapturedImageModel> GetCapturedImages(int applicationId);

    /// <summary>
    /// Get captured images for a specific process — semantics closer to 
    /// IProcessRepository.GetImagesForProcess(processId, page, pageSize).
    /// </summary>
    List<CapturedImageModel> GetCapturedImagesForProcess(int processId);
}