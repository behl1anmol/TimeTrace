using System.Linq.Expressions;
using timetrace.library.Context;
using timetrace.library.Models;

namespace timetrace.library.Repositories;
public class ProcessRepository : RepositoryBase, IProcessRepository
{
    private readonly IImageRepository _imageRepository;
    private readonly IConfigurationRepository _configurationRepository;
    public ProcessRepository(DatabaseContext dbContext
                            , IImageRepository imageRepository
                            , IConfigurationRepository configurationRepository) : base(dbContext)
    {
        _imageRepository = imageRepository;
        _configurationRepository = configurationRepository;
    }

    public Process AddProcess(Process process)
    {
        ValidateProcess(process);
        return base.Add(process);
    }

    public ProcessDetail AddProcessDetail(ProcessDetail processDetail)
    {
        ValidateProcessDetail(processDetail);
        return base.Add(processDetail);
    }

    public bool AddProcessDetails(List<ProcessDetail> processDetails)
    {
        foreach (var processDetail in processDetails)
        {
            ValidateProcessDetail(processDetail);
        }

        return base.AddAll(processDetails);
    }

    public bool AddProcesses(List<Process> processes)
    {
        foreach (var process in processes)
        {
            ValidateProcess(process);
        }

        return base.AddAll(processes);
    }

    public Process AddProcessWithDetails(Process process, List<ProcessDetail> processDetails)
    {
        ValidateProcess(process);
        AddProcess(process);
        AddProcessDetails(processDetails);
        return process;
    }

    public bool DeleteAllProcesses()
    {
        _imageRepository.DeleteAllImages();
        base.DeleteAll<ProcessDetail>();
        base.DeleteAll<Process>();
        return true;
    }

    public bool DeleteProcess(Process process)
    {
        try
        {
            Expression<Func<Image, bool>> expression = i => i.ProcessDetail.ProcessId == process.ProcessId;
            _imageRepository.DeleteImages(expression);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete process: {ex.Message}", ex);
        }
    }

    public bool DeleteProcessDetail(ProcessDetail processDetail)
    {
        throw new NotImplementedException();
    }

    public bool DeleteProcessDetails(List<ProcessDetail> processDetails)
    {
        throw new NotImplementedException();
    }

    public bool DeleteProcessDetailsByProcessId(int processId)
    {
        throw new NotImplementedException();
    }

    public bool DeleteProcesses(List<Process> processes)
    {
        throw new NotImplementedException();
    }

    public Process? GetProcess(int processId)
    {
        throw new NotImplementedException();
    }

    public Process? GetProcessByName(string name)
    {
        throw new NotImplementedException();
    }

    public ProcessDetail? GetProcessDetail(int processDetailId)
    {
        throw new NotImplementedException();
    }

    public List<ProcessDetail> GetProcessDetails(int processId, int page = 1, int pageSize = 100)
    {
        throw new NotImplementedException();
    }

    public List<ProcessDetail> GetProcessDetailsByDateRange(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 100)
    {
        throw new NotImplementedException();
    }

    public List<ProcessDetail> GetProcessDetailsWithImages(int processId, int page = 1, int pageSize = 100)
    {
        throw new NotImplementedException();
    }

    public List<Process> GetProcesses(int page = 1, int pageSize = 100)
    {
        throw new NotImplementedException();
    }

    public List<Process> GetProcessesWithDetails(int page = 1, int pageSize = 100)
    {
        throw new NotImplementedException();
    }

    public Process UpdateProcess(Process process)
    {
        throw new NotImplementedException();
    }

    public ProcessDetail UpdateProcessDetail(ProcessDetail processDetail)
    {
        throw new NotImplementedException();
    }

    public bool UpdateProcessDetails(List<ProcessDetail> processDetails)
    {
        throw new NotImplementedException();
    }

    public bool UpdateProcesses(List<Process> processes)
    {
        throw new NotImplementedException();
    }

    private static void ValidateProcess(Process process)
    {
        switch (process)
        {
            case { Name: var name } when string.IsNullOrWhiteSpace(name):
                throw new ArgumentException("Process name is required");
            case { Name: var name } when name.Length > 50:
                throw new ArgumentException("Process name cannot exceed 50 characters");
        }
    }

    private static void ValidateProcessDetail(ProcessDetail processDetail)
    {
        switch (processDetail)
        {
            case { ProcessId: 0 }:
                throw new ArgumentException("ProcessId is required");
            case { Description: var description } when string.IsNullOrWhiteSpace(description) || description.Length > 255:
                throw new ArgumentException(string.IsNullOrWhiteSpace(description) ? "Description is required" : "Description cannot exceed 255 characters");
        }
    }
}
