using System.Linq.Expressions;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
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

    public override TE Add<TE>(TE entity)
    {
        var savedImage = entity as Image;
        savedImage.ImageGuid = Guid.NewGuid();
        (savedImage.Name, savedImage.ImagePath) = CreateImagePathAndName(savedImage.ImageGuid);

        return base.Add(entity);
    }
    public Image AddImage(Image image)
    {
        var savedImage = this.Add(image);
        return base.Update(savedImage);
    }

    public bool AddImages(List<Image> images)
    {
        return base.AddAll(images);
    }
    public bool DeleteAllImages()
    {
        try
        {
            base.DeleteAll<Image>(base.DbContext.Images);
        }
        catch(Exception e)
        {
            throw new Exception(e.Message);
            return false;
        }
        return true;
    }
    public bool DeleteImage(Image image)
    {
        try
        {
            base.Delete(image);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
            return false;
        }
        return true;
    }
    public bool DeleteImages(List<Image> images)
    {
        try
        {
            base.DeleteAll(images);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
            return false;
        }
        return true;
    }
    public bool DeleteImages(int processDetailId)
    {
        try
        {
            base.DeleteAll<Image>(i => i.ProcessDetailId == processDetailId);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
            return false;
        }
        return true;

    }
    public List<Image> GetImages(Expression<Func<Image, bool>> expression, int page, int pageSize)
    {
        return base.FindAll(expression, pageSize, page).ToList();
    }
    public List<Image> GetImages(ProcessDetail processDetail, int page, int pageSize)
    {
        return base.FindAll<Image>(i => i.ProcessDetailId == processDetail.ProcessDetailId, pageSize, page).ToList();
    }
    public List<Image> GetImages(int processDetailId, int page, int pageSize)
    {
        return base.FindAll<Image>(i => i.ProcessDetailId == processDetailId, pageSize, page).ToList();
    }
    public List<Image> GetImages(List<int> processDetailId, int page, int pageSize)
    {
        return base.FindAll<Image>(i => processDetailId.Contains(i.ProcessDetailId), pageSize, page).ToList();
    }
    public (string fileName, string filePath) CreateImagePathAndName(Guid imageGuid)
    {
        var path = _configurationRepository.FetchConfigurationSettingValueByKeyIndex(ConfigSettingConstants.FilePathConfigSettingIndex, ConfigSettingConstants.ImagePathConfigSettingKey);

        var fileName = $"{imageGuid}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
        var filePath = $"{path}/{fileName}";
        return (fileName, filePath);
    }

    public bool UpdateAllImagePath()
    {
        const int batchSize = 1000;
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
}