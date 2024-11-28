using System.Linq.Expressions;
using timetrace.library.Context;
using timetrace.library.Models;
using timetrace.library.Utils;

namespace timetrace.library.Repositories;

public class ImageRepository : RepositoryBase, IImageRepository
{
    private readonly ConfigurationRepository _configurationRepository;
    public ImageRepository(DatabaseContext dbContext, ConfigurationRepository configurationRepository) : base(dbContext)
    {
        _configurationRepository = configurationRepository;
    }

    public Image AddImage(Image image)
    {
        image.ImageGuid = Guid.NewGuid();
        (image.Name, image.ImagePath) = CreateImagePathAndName(image.ImageGuid);

        ValidateForeignKeyRelations(image);
        return this.Add<Image>(image);
    }

    public bool AddImages(List<Image> images)
    {
        foreach (var image in images)
        {
            image.ImageGuid = Guid.NewGuid();
            (image.Name, image.ImagePath) = CreateImagePathAndName(image.ImageGuid);
            ValidateForeignKeyRelations(image);
        }
        return base.AddAll(images);
    }
    public bool DeleteAllImages()
    {
        base.DeleteAll<Image>();
        return true;
    }
    public bool DeleteImage(Image image)
    {
        base.Delete(image);
        return true;
    }
    public bool DeleteImages(List<Image> images)
    {
        base.DeleteAll(images);
        return true;
    }
    public bool DeleteImages(int processDetailId)
    {
        base.DeleteAll<Image>(i => i.ProcessDetailId == processDetailId);
        return true;
    }

    public bool DeleteImages(Expression<Func<Image, bool>> expression)
    {
        base.DeleteAll(expression);
        return true;
    }
    public List<Image> GetImages(Expression<Func<Image, bool>> expression, int page = 1, int pageSize = 100)
    {
        return base.FindAll(expression, pageSize, page);
    }
    public List<Image> GetImages(ProcessDetail processDetail, int page = 1, int pageSize = 100)
    {
        return base.FindAll<Image>(i => i.ProcessDetailId == processDetail.ProcessDetailId, pageSize, page);
    }
    public List<Image> GetImages(int processDetailId, int page = 1, int pageSize = 100)
    {
        return base.FindAll<Image>(i => i.ProcessDetailId == processDetailId, pageSize, page);
    }
    public List<Image> GetImages(List<int> processDetailId, int page = 1, int pageSize = 100)
    {
        return base.FindAll<Image>(i => processDetailId.Contains(i.ProcessDetailId), pageSize, page);
    }

    /// <summary>
    /// Gets the file path configuration setting value and creates a unique image path for the image using the current date and time.
    /// Image name can be passed as an optional parameter, if not passed, the image name will be a combination of the image guid and the current datetime.
    /// </summary>
    /// <param name="imageGuid"></param>
    /// <param name="imageName"></param>
    /// <returns></returns>
    private (string fileName, string filePath) CreateImagePathAndName(Guid imageGuid, string? imageName = null)
    {
        var path = _configurationRepository.FetchConfigurationSettingValueByKeyIndex(ConfigSettingConstants.FilePathConfigSettingIndex, ConfigSettingConstants.ImagePathConfigSettingKey);

        var fileName = imageName ?? $"{imageGuid}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
        var filePath = $"{path}/{fileName}";
        return (fileName, filePath);
    }

    public bool UpdateAllImagePath()
    {
        const int batchSize = 1000;
        try{
            var totalImages = base.FetchAll<Image>().Count();
            for (var i = 0; i < totalImages; i += batchSize)
            {
                var imagesBatch = base.FetchAll<Image>().Skip(i).Take(batchSize).ToList();
                foreach (var image in imagesBatch)
                {
                    (_, image.ImagePath) = CreateImagePathAndName(image.ImageGuid);
                }
                base.UpdateAll(imagesBatch);
            }
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    private void ValidateForeignKeyRelations(Image image)
    {
        if (image.ProcessDetailId == 0)
        {
            throw new Exception("ProcessDetailId is required");
        }
        if (!base.Exists<ProcessDetail>(pd=>pd.ProcessDetailId==image.ProcessDetailId))
        {
            throw new InvalidOperationException("Referenced Process Detail does not exists.");
        }
    }
}