using timetrace.ui.Models;

public interface IApplicationDataService
{
    List<ApplicationModel> GetApplications();
    List<CapturedImageModel> GetCapturedImages(int applicationId);
}