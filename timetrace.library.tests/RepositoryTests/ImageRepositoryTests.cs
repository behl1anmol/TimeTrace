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
    public void AddImage_ShouldAddNewImage(Image image)
    {
        // Act
        var result = _repository.AddImage(image);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ImageGuid, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.ImagePath, Is.Not.Null);
        Assert.That(1, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void AddImages_ShouldAddNewImages(List<Image> images)
    {
        // Act
        var result = _repository.AddImages(images);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(images.Count, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void DeleteImage_ShouldDeleteImage(Image image)
    {
        // Arrange
        _dbContext.Images.Add(image);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.DeleteImage(image);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(0, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void DeleteImages_ShouldDeleteImages(List<Image> images)
    {
        // Arrange
        _dbContext.Images.AddRange(images);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.DeleteImages(images);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(0, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void DeleteImagesByProcessDetailId_ShouldDeleteImages(int processDetailId, List<Image> images)
    {
        // Arrange
        images.ForEach(i => i.ProcessDetailId = processDetailId);
        _dbContext.Images.AddRange(images);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.DeleteImages(processDetailId);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(0, Is.EqualTo(_dbContext.Images.Count()));
    }

    [Test, CustomizedAutoData]
    public void GetImagesByExpression_ShouldReturnImages(Expression<Func<Image, bool>> expression, List<Image> images)
    {
        // Arrange
        _dbContext.Images.AddRange(images);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.GetImages(expression);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(images.Count));
    }

    [Test, CustomizedAutoData]
    public void GetImagesByProcessDetail_ShouldReturnImages(ProcessDetail processDetail, List<Image> images)
    {
        // Arrange
        images.ForEach(i => i.ProcessDetailId = processDetail.ProcessDetailId);
        _dbContext.Images.AddRange(images);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.GetImages(processDetail);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(images.Count));
    }

    [Test, CustomizedAutoData]
    public void GetImagesByProcessDetailId_ShouldReturnImages(int processDetailId, List<Image> images)
    {
        // Arrange
        images.ForEach(i => i.ProcessDetailId = processDetailId);
        _dbContext.Images.AddRange(images);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.GetImages(processDetailId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(images.Count));
    }

    [Test, CustomizedAutoData]
    public void GetImagesByProcessDetailIds_ShouldReturnImages(List<int> processDetailIds, List<Image> images)
    {
        // Arrange
        images.ForEach(i => i.ProcessDetailId = processDetailIds.First());
        _dbContext.Images.AddRange(images);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.GetImages(processDetailIds);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(images.Count));
    }

    [Test]
    public void UpdateAllImagePath_ShouldUpdateImagePaths()
    {
        // Arrange
        var images = new List<Image>
        {
            new Image { ImageGuid = Guid.NewGuid(), Name = "Image1", ProcessDetailId = 1 },
            new Image { ImageGuid = Guid.NewGuid(), Name = "Image2", ProcessDetailId = 1 }
        };
        _dbContext.Images.AddRange(images);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.UpdateAllImagePath();

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_dbContext.Images.All(i => i.ImagePath != null), Is.True);
    }
}
