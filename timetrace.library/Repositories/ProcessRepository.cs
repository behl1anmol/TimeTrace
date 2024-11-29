using System.Linq.Expressions;
using timetrace.library.Context;
using Microsoft.EntityFrameworkCore;
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
        foreach (var processDetail in processDetails)
        {
            processDetail.ProcessId = process.ProcessId;
            ValidateProcessDetail(processDetail);
        }
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
            Expression<Func<ProcessDetail, bool>> expression_pd = pd => pd.ProcessId == process.ProcessId;
            base.DeleteAll(expression_pd);
            base.Delete(process);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete process: {ex.Message}", ex);
        }
    }

    public bool DeleteProcess(int processId)
    {
        try
        {
            Expression<Func<Image, bool>> expression = i => i.ProcessDetail.ProcessId == processId;
            _imageRepository.DeleteImages(expression);
            Expression<Func<ProcessDetail, bool>> expression_pd = pd => pd.ProcessId == processId;
            base.DeleteAll(expression_pd);
            base.DeleteAll<Process>(p => p.ProcessId == processId);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete process: {ex.Message}", ex);
        }
    }

    public bool DeleteProcessDetail(ProcessDetail processDetail)
    {
        Expression<Func<Image, bool>> expression = i => i.ProcessDetailId == processDetail.ProcessDetailId;
        _imageRepository.DeleteImages(expression);
        base.Delete(processDetail);
        return true;
    }

    public bool DeleteProcessDetails(List<ProcessDetail> processDetails)
    {
        foreach (var processDetail in processDetails)
        {
            Expression<Func<Image, bool>> expression = i => i.ProcessDetailId == processDetail.ProcessDetailId;
            _imageRepository.DeleteImages(expression);
        }

        base.DeleteAll(processDetails);
        return true;
    }

    public bool DeleteProcessDetailsByProcessId(int processId)
    {
        Expression<Func<Image, bool>> expression = i => i.ProcessDetail.ProcessId == processId;
        _imageRepository.DeleteImages(expression);
        base.DeleteAll<ProcessDetail>(pd => pd.ProcessId == processId);
        return true;
    }

    public bool DeleteProcesses(List<Process> processes)
    {
        foreach (var process in processes)
        {
            DeleteProcess(process);
        }

        return true;
    }

    public Process? GetProcess(int processId)
    {
        return base.Find<Process>(p => p.ProcessId == processId);
    }

    public Process? GetProcessByName(string name)
    {
        return base.Find<Process>(p => p.Name == name);
    }

    public ProcessDetail? GetProcessDetail(int processDetailId)
    {
        return base.Find<ProcessDetail>(pd => pd.ProcessDetailId == processDetailId);
    }

    public List<ProcessDetail> GetProcessDetails(int processId, int page = 1, int pageSize = 100)
    {
        return base.FindAll<ProcessDetail>(pd => pd.ProcessId == processId, pageSize, page);
    }

    public List<ProcessDetail> GetProcessDetailsByDateRange(DateTime startDate, DateTime endDate, int page = 1, int pageSize = 100)
    {
        return base.FindAll<ProcessDetail>(pd => pd.DateTimeStamp >= startDate && pd.DateTimeStamp < endDate, pageSize, page);
    }

    public List<Image> GetImagesForProcess(int processId, int page = 1, int pageSize = 100)
    {
        return _imageRepository.GetImages(i => i.ProcessDetail.ProcessId == processId, page, pageSize);
    }

    public List<Process> GetProcesses(int page = 1, int pageSize = 100)
    {
        return base.FetchAll<Process>(pageSize, page);
    }

    public Process UpdateProcess(Process process)
    {
        ValidateProcess(process);
        return base.Update(process);
    }

    public ProcessDetail UpdateProcessDetail(ProcessDetail processDetail)
    {
        ValidateProcessDetail(processDetail);
        return base.Update(processDetail);
    }

    public bool UpdateProcessDetails(List<ProcessDetail> processDetails)
    {
        foreach (var processDetail in processDetails)
        {
            ValidateProcessDetail(processDetail);
        }

        base.UpdateAll(processDetails);
        return true;
    }

    public bool UpdateProcesses(List<Process> processes)
    {
        foreach (var process in processes)
        {
            ValidateProcess(process);
        }

        base.UpdateAll(processes);
        return true;
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
