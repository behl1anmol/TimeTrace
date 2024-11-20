using System.Linq.Expressions;
using timetrace.library.Models;

namespace timetrace.library.Repositories;
public interface IImageRepository : IRepositoryBase
{
    //create a function which will find images based on the func delegate passed from the process or process detail repository
    List<Image> GetImages(Expression<Func<Image, bool>> expression, int page, int pageSize);

    /// <summary>
    /// Retrieves a list of images associated with a specific process detail, with pagination support.
    /// </summary>
    /// <param name="processDetail">The process detail to retrieve images for.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of images per page.</param>
    /// <returns>A list of images.</returns>
    List<Image> GetImages(ProcessDetail processDetail, int page, int pageSize);

    /// <summary>
    /// Retrieves a list of images associated with a specific process detail ID, with pagination support.
    /// </summary>
    /// <param name="processDetailId">The ID of the process detail to retrieve images for.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of images per page.</param>
    /// <returns>A list of images.</returns>
    List<Image> GetImages(int processDetailId, int page, int pageSize);

    /// <summary>
    /// Retrieves a list of images associated with a list of process detail ID, with pagination support.
    /// </summary>
    /// <param name="processDetailId">The ID of the process detail to retrieve images for.</param>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of images per page.</param>
    /// <returns>A list of images.</returns>
    List<Image> GetImages(List<int> processDetailId, int page, int pageSize);

    /// <summary>
    /// Adds a new image to the repository.
    /// </summary>
    /// <param name="image">The image to add.</param>
    /// <returns>The added image.</returns>
    Image AddImage(Image image);

    /// <summary>
    /// Adds a list of images to the repository.
    /// </summary>
    /// <param name="images">The list of images to add.</param>
    /// <returns>True if the images were added successfully, false otherwise.</returns>
    bool AddImages(List<Image> images);

    ///// <summary>
    ///// Gets the file path configuration setting value and creates a unique image path for the image using the current date and time.
    ///// </summary>
    ///// <param name="imageId">The ID of the image.</param>
    ///// <returns>The image filename and image filepath.</returns>
    //(string fileName, string filePath) CreateImagePathAndName(Guid imageGuid);

    /// <summary>
    /// Deletes an image from the repository.
    /// </summary>
    /// <param name="image">The image to delete.</param>
    /// <returns>True if the image was deleted successfully, false otherwise.</returns>
    bool DeleteImage(Image image);

    /// <summary>
    /// Deletes a list of images from the repository.
    /// </summary>
    /// <param name="images">The list of images to delete.</param>
    /// <returns>True if the images were deleted successfully, false otherwise.</returns>
    bool DeleteImages(List<Image> images);

    /// <summary>
    /// Deletes all images associated with a specific process detail.
    /// </summary>
    /// <param name="processDetailId">The ID of the process detail.</param>
    /// <returns>True if the images were deleted successfully, false otherwise.</returns>
    bool DeleteImages(int processDetailId);

    /// <summary>
    /// Deletes all images from the Database.
    /// </summary>
    /// <returns>True if all images were deleted successfully, false otherwise.</returns>
    bool DeleteAllImages();

    /// <summary>
    /// Updates the image path for all images in the database.
    /// </summary>
    /// <returns>True if the image paths were updated successfully, false otherwise.</returns>
    bool UpdateAllImagePath();
}
