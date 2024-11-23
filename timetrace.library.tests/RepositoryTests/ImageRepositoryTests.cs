using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using timetrace.library.Context;
using timetrace.library.Models;
using timetrace.library.Repositories;
using timetrace.library.tests.TestHelpers;

namespace timetrace.library.tests.RepositoryTests;

[TestCategory(nameof(ImageRepositoryTests))]
[TestFixture]
public class ImageRepositoryTests
{
    private DatabaseContext _dbContext;
    private ImageRepository _repository;
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
        _repository = new ImageRepository(_dbContext, _configurationRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.CloseConnection();
        _dbContext.Dispose();
    }

    [Test, CustomizedAutoData]
    public void AddImage_ShouldAddNewImage(Image image, Process process, ProcessDetail processDetail)
    {
        // Arrange
        processDetail.ProcessId = process.ProcessId;
        image.ProcessDetailId = processDetail.ProcessDetailId;
        _dbContext.Processes.Add(process);
        _dbContext.ProcessDetails.Add(processDetail);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.AddImage(image);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ImageGuid, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.ImagePath, Is.Not.Null);
        Assert.That(1, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void AddImages_ShouldAddNewImages(List<Image> images, Process process, ProcessDetail processDetail)
    {
        // Arrange
        processDetail.ProcessId = process.ProcessId;
        images.ForEach(i=>i.ProcessDetailId = processDetail.ProcessDetailId);
        _dbContext.Processes.Add(process);
        _dbContext.ProcessDetails.Add(processDetail);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.AddImages(images);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(images.Count, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void DeleteImage_ShouldDeleteImage(Image image, Process process, ProcessDetail processDetail)
    {
        // Arrange
        SetupMockData(new List<Image> {image},
                    process,
                    processDetail
        );

        // Act
        var result = _repository.DeleteImage(image);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(0, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void DeleteImages_ShouldDeleteImages(List<Image> images, Process process, ProcessDetail processDetail)
    {
        // Arrange
        SetupMockData(images,
                    process,
                    processDetail
        );

        // Act
        var result = _repository.DeleteImages(images);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(0, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void DeleteImagesByProcessDetailId_ShouldDeleteImages(List<Image> images, Process process, ProcessDetail processDetail)
    {
        // Arrange
        SetupMockData(images,
                    process,
                   processDetail
        );

        // Act
        var result = _repository.DeleteImages(processDetail.ProcessDetailId);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(0, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void GetImagesByExpression_ShouldReturnImages(List<Image> images, Process process, ProcessDetail processDetail)
    {
        // Arrange
        SetupMockData(images,
                    process,
                    processDetail
        );
        Expression<Func<Image, bool>> expression = i => i.ProcessDetailId == processDetail.ProcessDetailId;

        //get something from mock data
        var process1 = _dbContext.Processes.First();
        // Act
        var result = _repository.GetImages(expression);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(images.Count));
    }

    [Test, CustomizedAutoData]
    public void GetImagesByProcessDetail_ShouldReturnImages(ProcessDetail processDetail, List<Image> images, Process process)
    {
        // Arrange
        SetupMockData(images,
                    process,
                    processDetail
        );

        // Act
        var result = _repository.GetImages(processDetail);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(images.Count));
    }

    [Test, CustomizedAutoData]
    public void GetImagesByProcessDetailId_ShouldReturnImages(List<Image> images, Process process, ProcessDetail processDetail)
    {
        // Arrange
        SetupMockData(images,
                    process,
                    processDetail
        );

        // Act
        var result = _repository.GetImages(processDetail.ProcessDetailId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(images.Count));
    }

    [Test, CustomizedAutoData]
    public void GetImagesByProcessDetailIds_ShouldReturnImages(List<Image> images, Process process, List<ProcessDetail> processDetail)
    {
        // Arrange
        processDetail.ForEach(pd => pd.ProcessId = process.ProcessId);
        foreach(var image in images)
        {
            image.ProcessDetailId = processDetail[new Random().Next(0, processDetail.Count)].ProcessDetailId;
        }

        var processDetailIds = processDetail.Select(pd => pd.ProcessDetailId).ToList();
        _dbContext.Processes.Add(process);
        _dbContext.ProcessDetails.AddRange(processDetail);
        _dbContext.Images.AddRange(images);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.GetImages(processDetailIds);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(images.Count));
    }

    [Test, CustomizedAutoData]
    public void UpdateAllImagePath_ShouldUpdateImagePaths(Process process, ProcessDetail processDetail, List<Image> images)
    {
        // Arrange
        SetupMockData(images,
                    process,
                    processDetail
        );
        // Act
        var result = _repository.UpdateAllImagePath();

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.Images.All(i => i.ImagePath != null), Is.True);
    }

    private void SetupMockData(List<Image> images, Process process, ProcessDetail processDetails)
    {
        processDetails.ProcessId = process.ProcessId;
        images.ForEach(i => i.ProcessDetailId = processDetails.ProcessDetailId);
        _dbContext.Processes.Add(process);
        _dbContext.ProcessDetails.AddRange(processDetails);
        _dbContext.Images.AddRange(images);
        _dbContext.SaveChanges();
    }
}
