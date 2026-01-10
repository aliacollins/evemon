using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using EVEMon.Common;
using EVEMon.Common.Constants;

namespace EVEMon.Avalonia.Services;

/// <summary>
/// Service for loading and caching images in Avalonia.
/// </summary>
public class AvaloniaImageService
{
    private static readonly Lazy<AvaloniaImageService> _instance = new(() => new AvaloniaImageService());
    private static readonly HttpClient _httpClient = new();
    private readonly ConcurrentDictionary<string, Task<Bitmap?>> _loadingTasks = new();
    private readonly ConcurrentDictionary<string, Bitmap?> _memoryCache = new();

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static AvaloniaImageService Instance => _instance.Value;

    private AvaloniaImageService()
    {
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Gets a character portrait asynchronously.
    /// </summary>
    /// <param name="characterId">The character ID.</param>
    /// <param name="size">The size (32, 64, 128, 256, 512, 1024).</param>
    /// <returns>The portrait bitmap or null if not available.</returns>
    public async Task<Bitmap?> GetCharacterPortraitAsync(long characterId, int size = 128)
    {
        if (characterId <= 0)
            return null;

        string path = string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            NetworkConstants.CCPPortraits,
            characterId,
            size);

        var url = new Uri(NetworkConstants.EVEImageServerBase + path);
        return await GetImageAsync(url);
    }

    /// <summary>
    /// Gets a corporation logo asynchronously.
    /// </summary>
    /// <param name="corporationId">The corporation ID.</param>
    /// <param name="size">The size.</param>
    /// <returns>The logo bitmap or null if not available.</returns>
    public async Task<Bitmap?> GetCorporationLogoAsync(long corporationId, int size = 64)
    {
        if (corporationId <= 0)
            return null;

        string path = string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            NetworkConstants.CCPCorporationLogo,
            corporationId,
            size);

        var url = new Uri(NetworkConstants.EVEImageServerBase + path);
        return await GetImageAsync(url);
    }

    /// <summary>
    /// Gets an alliance logo asynchronously.
    /// </summary>
    /// <param name="allianceId">The alliance ID.</param>
    /// <param name="size">The size.</param>
    /// <returns>The logo bitmap or null if not available.</returns>
    public async Task<Bitmap?> GetAllianceLogoAsync(long allianceId, int size = 64)
    {
        if (allianceId <= 0)
            return null;

        string path = string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            NetworkConstants.CCPAllianceLogo,
            allianceId,
            size);

        var url = new Uri(NetworkConstants.EVEImageServerBase + path);
        return await GetImageAsync(url);
    }

    /// <summary>
    /// Gets a type icon asynchronously.
    /// </summary>
    /// <param name="typeId">The type ID.</param>
    /// <param name="size">The size.</param>
    /// <returns>The icon bitmap or null if not available.</returns>
    public async Task<Bitmap?> GetTypeIconAsync(int typeId, int size = 64)
    {
        if (typeId <= 0)
            return null;

        string path = string.Format(
            System.Globalization.CultureInfo.InvariantCulture,
            NetworkConstants.CCPTypeImage,
            typeId,
            size);

        var url = new Uri(NetworkConstants.EVEImageServerBase + path);
        return await GetImageAsync(url);
    }

    /// <summary>
    /// Gets an image from a URL with caching.
    /// </summary>
    /// <param name="url">The image URL.</param>
    /// <returns>The bitmap or null if not available.</returns>
    public async Task<Bitmap?> GetImageAsync(Uri url)
    {
        string cacheKey = GetCacheKey(url);

        // Check memory cache first
        if (_memoryCache.TryGetValue(cacheKey, out var cachedBitmap))
        {
            return cachedBitmap;
        }

        // Check if already loading
        if (_loadingTasks.TryGetValue(cacheKey, out var loadingTask))
        {
            return await loadingTask;
        }

        // Start loading
        var task = LoadImageAsync(url, cacheKey);
        _loadingTasks[cacheKey] = task;

        try
        {
            var result = await task;
            _memoryCache[cacheKey] = result;
            return result;
        }
        finally
        {
            _loadingTasks.TryRemove(cacheKey, out _);
        }
    }

    private async Task<Bitmap?> LoadImageAsync(Uri url, string cacheKey)
    {
        // Try disk cache first
        var cachedImage = await LoadFromDiskCacheAsync(cacheKey);
        if (cachedImage != null)
        {
            return cachedImage;
        }

        // Download from network
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Trace.WriteLine($"Failed to download image: {url} - {response.StatusCode}");
                return null;
            }

            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            if (imageBytes.Length == 0)
            {
                return null;
            }

            // Save to disk cache
            await SaveToDiskCacheAsync(cacheKey, imageBytes);

            // Create bitmap
            using var stream = new MemoryStream(imageBytes);
            return new Bitmap(stream);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"Error loading image {url}: {ex.Message}");
            return null;
        }
    }

    private async Task<Bitmap?> LoadFromDiskCacheAsync(string cacheKey)
    {
        try
        {
            string cacheDir = GetCacheDirectory();
            string cachePath = Path.Combine(cacheDir, cacheKey + ".png");

            if (!File.Exists(cachePath))
            {
                return null;
            }

            var imageBytes = await File.ReadAllBytesAsync(cachePath);
            using var stream = new MemoryStream(imageBytes);
            return new Bitmap(stream);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"Error loading from disk cache: {ex.Message}");
            return null;
        }
    }

    private async Task SaveToDiskCacheAsync(string cacheKey, byte[] imageBytes)
    {
        try
        {
            string cacheDir = GetCacheDirectory();
            Directory.CreateDirectory(cacheDir);

            string cachePath = Path.Combine(cacheDir, cacheKey + ".png");
            await File.WriteAllBytesAsync(cachePath, imageBytes);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"Error saving to disk cache: {ex.Message}");
        }
    }

    private static string GetCacheDirectory()
    {
        // Use EVEMon's existing cache directory if available
        try
        {
            var cacheDir = EveMonClient.EVEMonImageCacheDir;
            if (!string.IsNullOrEmpty(cacheDir))
            {
                return cacheDir;
            }
        }
        catch
        {
            // Ignore - use fallback
        }

        // Fallback to temp directory
        return Path.Combine(Path.GetTempPath(), "EVEMon", "ImageCache");
    }

    private static string GetCacheKey(Uri url)
    {
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(url.AbsoluteUri));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// Clears the memory cache.
    /// </summary>
    public void ClearMemoryCache()
    {
        _memoryCache.Clear();
    }
}
