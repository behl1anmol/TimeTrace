using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace timetrace.ui.avalonia.Services;

/// <summary>
/// Service for loading images from disk with caching and thumbnail generation
/// </summary>
public interface IImageLoadingService
{
    Task<Bitmap?> LoadImageAsync(string filePath);
    Task<Bitmap?> LoadThumbnailAsync(string filePath, int maxWidth = 200);
    void ClearCache();
}

public class ImageLoadingService : IImageLoadingService
{
    private readonly Dictionary<string, WeakReference<Bitmap>> _cache = new();
    private readonly object _lock = new();

    public async Task<Bitmap?> LoadImageAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return null;

        try
        {
            // Check cache first
            lock (_lock)
            {
                if (_cache.TryGetValue(filePath, out var weakRef) && weakRef.TryGetTarget(out var cachedBitmap))
                {
                    return cachedBitmap;
                }
            }

            // Load from disk
            return await Task.Run(() =>
            {
                using var stream = File.OpenRead(filePath);
                var bitmap = new Bitmap(stream);
                
                // Cache with weak reference
                lock (_lock)
                {
                    _cache[filePath] = new WeakReference<Bitmap>(bitmap);
                }

                return bitmap;
            });
        }
        catch (Exception)
        {
            // Return null for failed loads - caller handles placeholder
            return null;
        }
    }

    public async Task<Bitmap?> LoadThumbnailAsync(string filePath, int maxWidth = 200)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return null;

        var cacheKey = $"{filePath}:thumb:{maxWidth}";

        try
        {
            // Check cache first
            lock (_lock)
            {
                if (_cache.TryGetValue(cacheKey, out var weakRef) && weakRef.TryGetTarget(out var cachedBitmap))
                {
                    return cachedBitmap;
                }
            }

            // Load and decode to width
            return await Task.Run(() =>
            {
                using var stream = File.OpenRead(filePath);
                var bitmap = Bitmap.DecodeToWidth(stream, maxWidth);
                
                // Cache with weak reference
                lock (_lock)
                {
                    _cache[cacheKey] = new WeakReference<Bitmap>(bitmap);
                }

                return bitmap;
            });
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void ClearCache()
    {
        lock (_lock)
        {
            _cache.Clear();
        }
    }
}
