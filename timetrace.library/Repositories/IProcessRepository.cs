using timetrace.library.Models;

namespace timetrace.library.Repositories;

public interface IProcessRepository : IRepositoryBase
{
    // Process Methods
    Process AddProcess(Process process);
    bool AddProcesses(List<Process> processes);
    bool DeleteProcess(Process process);
    bool DeleteProcess(int processId);
    bool DeleteProcesses(List<Process> processes);
    bool DeleteAllProcesses();
    Process? GetProcess(int processId);
    Process? GetProcessByName(string name); 
    List<Process> GetProcesses(int page = 1, int pageSize = 100);
    List<Process> GetProcessesWithDetails(int page = 1, int pageSize = 100);
    Process UpdateProcess(Process process);
    bool UpdateProcesses(List<Process> processes);

    // ProcessDetail Methods
    ProcessDetail AddProcessDetail(ProcessDetail processDetail);
    bool AddProcessDetails(List<ProcessDetail> processDetails);
    bool DeleteProcessDetail(ProcessDetail processDetail);
    bool DeleteProcessDetails(List<ProcessDetail> processDetails);
    bool DeleteProcessDetailsByProcessId(int processId);
    ProcessDetail? GetProcessDetail(int processDetailId);
    List<ProcessDetail> GetProcessDetails(int processId, int page = 1, int pageSize = 100);
    List<ProcessDetail> GetProcessDetailsByDateRange(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 100);
    ProcessDetail UpdateProcessDetail(ProcessDetail processDetail);
    bool UpdateProcessDetails(List<ProcessDetail> processDetails);

    // Combined Operations
    Process AddProcessWithDetails(Process process, List<ProcessDetail> processDetails);
    List<Image> GetImagesForProcess(int processId, int page = 1, int pageSize = 100);
}
