using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using timetrace.library.Context;
using timetrace.library.Models;
using timetrace.library.Repositories;
using timetrace.library.tests.TestHelpers;

namespace timetrace.library.tests.RepositoryTests;

[TestCategory(nameof(ProcessRepositoryTests))]
[TestFixture]
public class ProcessRepositoryTests
{
    private DatabaseContext _dbContext;
    private ProcessRepository _repository;
    private ImageRepository _imageRepository;
    private ConfigurationRepository _configurationRepository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite("DataSource=:memory:")
            .EnableSensitiveDataLogging()
            .Options;

        _dbContext = new DatabaseContext(options);
        _dbContext.Database.OpenConnection();
        _dbContext.Database.EnsureCreated();
        _configurationRepository = new ConfigurationRepository(_dbContext);
        _imageRepository = new ImageRepository(_dbContext, _configurationRepository);
        _repository = new ProcessRepository(_dbContext, _imageRepository, _configurationRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.CloseConnection();
        _dbContext.Dispose();
    }

    [Test, CustomizedAutoData]
    public void AddProcess_ShouldAddNewProcess(Process process)
    {
        // Act
        var result = _repository.AddProcess(process);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProcessId, Is.Not.EqualTo(0));
        Assert.That(_dbContext.Processes.Count(), Is.EqualTo(1));
    }

    [Test, CustomizedAutoData]
    public void AddProcessDetail_ShouldAddNewProcessDetail(ProcessDetail processDetail, Process process)
    {
        //Arrange
        _repository.AddProcess(process);
        processDetail.ProcessId = process.ProcessId;

        // Act
        var result = _repository.AddProcessDetail(processDetail);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProcessDetailId, Is.Not.EqualTo(0));
        Assert.That(_dbContext.ProcessDetails.Count(), Is.EqualTo(1));
    }

    [Test, CustomizedAutoData]
    public void AddProcessDetails_ShouldAddNewProcessDetails(List<ProcessDetail> processDetails, Process process)
    {
        //Arrange
        _repository.AddProcess(process);
        processDetails.ForEach(pd => pd.ProcessId = process.ProcessId);

        // Act
        var result = _repository.AddProcessDetails(processDetails);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(processDetails.Count, Is.EqualTo(_dbContext.ProcessDetails.Count()));
    }

    [Test, CustomizedAutoData]
    public void AddProcesses_ShouldAddNewProcesses(List<Process> processes)
    {
        // Act
        var result = _repository.AddProcesses(processes);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(processes.Count, Is.EqualTo(_dbContext.Processes.Count()));
    }

    [Test, CustomizedAutoData]
    public void AddProcessWithDetails_ShouldAddNewProcessWithDetails(Process process, List<ProcessDetail> processDetails)
    {
        // Act
        var result = _repository.AddProcessWithDetails(process, processDetails);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProcessId, Is.Not.EqualTo(0));
        Assert.That(_dbContext.Processes.Count(), Is.EqualTo(1));
        Assert.That(processDetails.Count, Is.EqualTo(_dbContext.ProcessDetails.Count()));
    }

    [Test, CustomizedAutoData]
    public void DeleteAllProcesses_ShouldDeleteAllProcesses()
    {
        // Arrange
        var processes = new List<Process> { new Process { Name = "Test Process" } };
        _repository.AddProcesses(processes);

        // Act
        var result = _repository.DeleteAllProcesses();

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.Processes.Count(), Is.EqualTo(0));
    }

    [Test, CustomizedAutoData]
    public void DeleteProcess_ShouldDeleteProcess(Process process)
    {
        // Arrange
        _repository.AddProcess(process);

        // Act
        var result = _repository.DeleteProcess(process);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.Processes.Count(), Is.EqualTo(0));
    }

    [Test, CustomizedAutoData]
    public void DeleteProcessById_ShouldDeleteProcess(int processId)
    {
        // Arrange
        var process = new Process { ProcessId = processId, Name = "Test Process" };
        _repository.AddProcess(process);

        // Act
        var result = _repository.DeleteProcess(processId);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.Processes.Count(), Is.EqualTo(0));
    }

    [Test, CustomizedAutoData]
    public void DeleteProcessDetail_ShouldDeleteProcessDetail(ProcessDetail processDetail, Process process)
    {
        // Arrange
        _repository.AddProcess(process);
        processDetail.ProcessId = process.ProcessId;
        _repository.AddProcessDetail(processDetail);

        // Act
        var result = _repository.DeleteProcessDetail(processDetail);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.ProcessDetails.Count(), Is.EqualTo(0));
    }

    [Test, CustomizedAutoData]
    public void DeleteProcessDetails_ShouldDeleteProcessDetails(List<ProcessDetail> processDetails, Process process)
    {
        // Arrange
        _repository.AddProcess(process);
        processDetails.ForEach(pd => pd.ProcessId = process.ProcessId);
        _repository.AddProcessDetails(processDetails);

        // Act
        var result = _repository.DeleteProcessDetails(processDetails);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.ProcessDetails.Count(), Is.EqualTo(0));
    }

    [Test, CustomizedAutoData]
    public void DeleteProcessDetailsByProcessId_ShouldDeleteProcessDetailsByProcessId(Process process)
    {
        // Arrange
        _repository.AddProcess(process);
        var processDetails = new List<ProcessDetail> { new ProcessDetail { ProcessId = process.ProcessId, Description = "Test Detail" } };
        _repository.AddProcessDetails(processDetails);

        // Act
        var result = _repository.DeleteProcessDetailsByProcessId(process.ProcessId);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.ProcessDetails.Count(), Is.EqualTo(0));
    }

    [Test, CustomizedAutoData]
    public void DeleteProcesses_ShouldDeleteProcesses(List<Process> processes)
    {
        // Arrange
        _repository.AddProcesses(processes);

        // Act
        var result = _repository.DeleteProcesses(processes);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.Processes.Count(), Is.EqualTo(0));
    }

    [Test, CustomizedAutoData]
    public void GetProcess_ShouldReturnProcess(int processId)
    {
        // Arrange
        var process = new Process { ProcessId = processId, Name = "Test Process" };
        _repository.AddProcess(process);

        // Act
        var result = _repository.GetProcess(processId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProcessId, Is.EqualTo(processId));
    }

    [Test, CustomizedAutoData]
    public void GetProcessByName_ShouldReturnProcess(string name)
    {
        // Arrange
        var process = new Process { Name = name };
        _repository.AddProcess(process);

        // Act
        var result = _repository.GetProcessByName(name);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(name));
    }

    [Test, CustomizedAutoData]
    public void GetProcessDetail_ShouldReturnProcessDetail(Process process)
    {
        // Arrange
        _repository.AddProcess(process);
        var processDetail = new ProcessDetail { ProcessId = process.ProcessId, Description = "Test Detail" };
        var savedEntity = _repository.AddProcessDetail(processDetail);

        // Act
        var result = _repository.GetProcessDetail(savedEntity.ProcessDetailId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProcessDetailId, Is.EqualTo(savedEntity.ProcessDetailId));
    }

    [Test, CustomizedAutoData]
    public void GetProcessDetails_ShouldReturnProcessDetails(Process process)
    {
        // Arrange
        _repository.AddProcess(process);
        var processDetails = new List<ProcessDetail> { new ProcessDetail { ProcessId = process.ProcessId, Description = "Test Detail" } };
        _repository.AddProcessDetails(processDetails);

        // Act
        var result = _repository.GetProcessDetails(process.ProcessId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(processDetails.Count));
    }

    [Test, CustomizedAutoData]
    public void GetProcessDetailsByDateRange_ShouldReturnProcessDetailsByDateRange(Process process)
    {
        // Arrange
        _repository.AddProcess(process);
        var processDetails = new List<ProcessDetail> { new ProcessDetail { ProcessId = process.ProcessId, Description = "Test Detail" } };
        _repository.AddProcessDetails(processDetails);
        var startDate = DateTime.Now;
        var endDate = DateTime.Now.AddDays(2);

        // Act
        var result = _repository.GetProcessDetailsByDateRange(startDate, endDate);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(processDetails.Count));
    }

    [Test, CustomizedAutoData]
    public void GetImagesForProcess_ShouldReturnImagesForProcess(Process process, ProcessDetail processDetail, List<Image> images)
    {
        // Arrange
        _repository.AddProcess(process);
        processDetail.ProcessId = process.ProcessId;
        _repository.AddProcessDetail(processDetail);
        images.ForEach(i => i.ProcessDetailId = processDetail.ProcessDetailId);
        _imageRepository.AddImages(images);

        // Act
        var result = _repository.GetImagesForProcess(process.ProcessId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(images.Count));
    }

    [Test, CustomizedAutoData]
    public void GetProcesses_ShouldReturnProcesses()
    {
        // Arrange
        var processes = new List<Process> { new Process { Name = "Test Process" } };
        _repository.AddProcesses(processes);

        // Act
        var result = _repository.GetProcesses();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(processes.Count));
    }

    [Test, CustomizedAutoData]
    public void UpdateProcess_ShouldUpdateProcess(Process process)
    {
        // Arrange
        _repository.AddProcess(process);
        process.Name = "Updated Process";

        // Act
        var result = _repository.UpdateProcess(process);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Updated Process"));
    }

    [Test, CustomizedAutoData]
    public void UpdateProcessDetail_ShouldUpdateProcessDetail(ProcessDetail processDetail, Process process)
    {
        // Arrange
        _repository.AddProcess(process);
        processDetail.ProcessId = process.ProcessId;
        _repository.AddProcessDetail(processDetail);
        processDetail.Description = "Updated Detail";

        // Act
        var result = _repository.UpdateProcessDetail(processDetail);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Description, Is.EqualTo("Updated Detail"));
    }

    [Test, CustomizedAutoData]
    public void UpdateProcessDetails_ShouldUpdateProcessDetails(List<ProcessDetail> processDetails, Process process)
    {
        // Arrange
        _repository.AddProcess(process);
        processDetails.ForEach(pd => pd.ProcessId = process.ProcessId);
        _repository.AddProcessDetails(processDetails);
        processDetails.ForEach(pd => pd.Description = "Updated Detail");

        // Act
        var result = _repository.UpdateProcessDetails(processDetails);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.ProcessDetails.All(pd => pd.Description == "Updated Detail"), Is.True);
    }

    [Test, CustomizedAutoData]
    public void UpdateProcesses_ShouldUpdateProcesses(List<Process> processes)
    {
        // Arrange
        _repository.AddProcesses(processes);
        for(int i = 0; i < processes.Count; i++)
        {
            processes[i].Name = $"Updated Process {i}";
        }

        // Act
        var result = _repository.UpdateProcesses(processes);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test, CustomizedAutoData]
    public void UpdateProcesses_ShouldFailUpdateProcessesThrowException(List<Process> processes)
    {
        // Arrange
        _repository.AddProcesses(processes);
        processes.ForEach(p => p.Name = "Updated Name");

        // Act
        void action() => _repository.UpdateProcesses(processes);

        // Assert
        Assert.That(action, Throws.Exception);
    }

}
