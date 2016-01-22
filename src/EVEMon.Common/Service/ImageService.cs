using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Helpers;
using EVEMon.Common.Net;
using HttpWebClientService = EVEMon.Common.Net.HttpWebClientService;

namespace EVEMon.Common.Service
{
    public static class ImageService
    {
        /// <summary>
        /// Gets the image server CDN URI.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Uri GetImageServerCdnUri(string path)
            => new Uri(
                String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEImageServerCDN, path));

        /// <summary>
        /// Gets the image server base URI.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Uri GetImageServerBaseUri(string path)
            => new Uri(
                String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEImageServerBase, path));

        /// <summary>
        /// Asynchronously downloads a character portrait from its ID.
        /// </summary>
        /// <param name="charId"></param>
        internal static async Task<Image> GetCharacterImageAsync(long charId)
        {
            string path = String.Format(CultureConstants.InvariantCulture,
                NetworkConstants.CCPPortraits, charId, (int)EveImageSize.x128);

            return await GetImageAsync(GetImageServerCdnUri(path), false) ??
                   await GetImageAsync(GetImageServerBaseUri(path), false);
        }

        /// <summary>
        /// Asynchronously downloads an alliance image.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        /// <param name="allianceID">The alliance ID.</param>
        public static void GetAllianceImage(PictureBox pictureBox, long allianceID)
        {
            string path = String.Format(CultureConstants.InvariantCulture, NetworkConstants.CCPIconsFromImageServer,
                "alliance", allianceID, pictureBox.Width);

            GetImageAsync(pictureBox, path);
        }

        /// <summary>
        /// Asynchronously downloads a corporation image.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        /// <param name="corporationID">The corporation ID.</param>
        public static void GetCorporationImage(PictureBox pictureBox, long corporationID)
        {
            string path = String.Format(CultureConstants.InvariantCulture, NetworkConstants.CCPIconsFromImageServer,
                "corporation", corporationID, pictureBox.Width);

            GetImageAsync(pictureBox, path);
        }

        /// <summary>
        /// Called when image gets downloaded.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        /// <param name="path">The path.</param>
        private static async void GetImageAsync(PictureBox pictureBox, string path)
        {
            Image image = await GetImageAsync(GetImageServerCdnUri(path)) ??
                        await GetImageAsync(GetImageServerBaseUri(path));

            pictureBox.Image = image ?? pictureBox.InitialImage;
            pictureBox.Update();
        }

        /// <summary>
        /// Asynchronously downloads an image from the provided url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="useCache">if set to <c>true</c> [use cache].</param>
        public static async Task<Image> GetImageAsync(Uri url, bool useCache = true)
        {
            DownloadAsyncResult<Image> result;

            // Cache not to be used ?
            if (!useCache)
            {
                result = await HttpWebClientService.DownloadImageAsync(url);
                return GotImage(result);
            }

            // First check whether the image exists in cache
            EveMonClient.EnsureCacheDirInit();
            string cacheFileName = Path.Combine(EveMonClient.EVEMonImageCacheDir, await GetCacheName(url));

            if (File.Exists(cacheFileName))
            {
                try
                {
                    // Load the data into a MemoryStream
                    // before returning the image
                    // to avoid file locking
                    Image image;

                    byte[] imageBytes = File.ReadAllBytes(cacheFileName);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Write(imageBytes, 0, imageBytes.Length);
                        stream.Position = 0;

                        image = Image.FromStream(stream);
                    }
                    return image;
                }
                catch (ArgumentException e)
                {
                    ExceptionHandler.LogException(e, false);
                    File.Delete(cacheFileName);
                }
                catch (IOException e)
                {
                    ExceptionHandler.LogException(e, false);
                }
                catch (UnauthorizedAccessException e)
                {
                    ExceptionHandler.LogException(e, false);
                }
            }

            // Downloads the image and adds it to cache
            result = await HttpWebClientService.DownloadImageAsync(url);
            Image img = GotImage(result);

            if (img != null)
                await AddImageToCache(url, img);

            return img;
        }

        /// <summary>
        /// Asynchronously gets the character image from cache.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns></returns>
        public static async Task<Image> GetCharacterImageFromCacheAsync(Guid guid)
            => await Task.Run(() =>
            {
                // First check whether the image exists in cache
                EveMonClient.EnsureCacheDirInit();
                string cacheFileName = Path.Combine(EveMonClient.EVEMonPortraitCacheDir, $"{guid}.png");

                if (!File.Exists(cacheFileName))
                    return null;

                try
                {
                    // Load the data into a MemoryStream
                    // before returning the image
                    // to avoid file locking
                    Image image;

                    byte[] imageBytes = File.ReadAllBytes(cacheFileName);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Write(imageBytes, 0, imageBytes.Length);
                        stream.Position = 0;

                        image = Image.FromStream(stream);
                    }
                    return image;
                }
                catch (ArgumentException e)
                {
                    ExceptionHandler.LogException(e, false);
                    File.Delete(cacheFileName);
                }
                catch (IOException e)
                {
                    ExceptionHandler.LogException(e, false);
                }
                catch (UnauthorizedAccessException e)
                {
                    ExceptionHandler.LogException(e, false);
                }

                return null;
            });

        /// <summary>
        /// Callback used when images are downloaded, it takes care to invoke another callback provided as our user state.
        /// </summary>
        /// <param name="result">The result.</param>
        private static Image GotImage(DownloadAsyncResult<Image> result)
        {
            if (result.Error == null)
                return result.Result;

            if (result.Error.Status == HttpWebClientServiceExceptionStatus.Timeout)
                EveMonClient.Trace($"{typeof(ImageService).Name}: {result.Error.Message}");
            else
                ExceptionHandler.LogException(result.Error, true);

            return null;
        }

        /// <summary>
        /// Adds the image to the cache.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="image"></param>
        private static async Task AddImageToCache(Uri url, Image image)
        {
            await Task.Run(async () =>
            {
                // Saves the image file
                try
                {
                    // Write this image to the cache file
                    EveMonClient.EnsureCacheDirInit();
                    string cacheFileName = Path.Combine(EveMonClient.EVEMonImageCacheDir, await GetCacheName(url));
                    FileHelper.OverwriteOrWarnTheUser(cacheFileName,
                        fs =>
                        {
                            // We need to create a copy of the image because GDI+ is locking it
                            using (Image newImage = new Bitmap(image))
                            {
                                newImage.Save(fs, ImageFormat.Png);
                                fs.Flush();
                            }
                            return true;
                        });
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogRethrowException(ex);
                    throw;
                }
            });
        }

        /// <summary>
        /// Adds the portrait to the cache.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        internal static async Task AddCharacterImageToCache(Guid guid, Image image)
        {
            await Task.Run(() =>
            {
                // Saves the image file
                try
                {
                    // Save the image to the portrait cache file
                    EveMonClient.EnsureCacheDirInit();
                    string cacheFileName = Path.Combine(EveMonClient.EVEMonPortraitCacheDir, $"{guid}.png");
                    FileHelper.OverwriteOrWarnTheUser(cacheFileName,
                        fs =>
                        {
                            // We need to create a copy of the image because GDI+ is locking it
                            using (Image newImage = new Bitmap(image))
                            {
                                newImage.Save(fs, ImageFormat.Png);
                                fs.Flush();
                            }
                            return true;
                        });
                }
                catch (Exception ex)
                {
                    ExceptionHandler.LogRethrowException(ex);
                    throw;
                }
            });
        }

        /// <summary>
        /// From a given url, computes a cache file name.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task<string> GetCacheName(Uri url)
            => await Task.Run(() =>
            {
                Match extensionMatch = Regex.Match(url.AbsoluteUri, @"([^\.]+)$");
                string ext = String.Empty;
                if (extensionMatch.Success)
                    ext = "." + extensionMatch.Groups[1];

                Stream stream = Util.GetMemoryStream(Encoding.UTF8.GetBytes(url.AbsoluteUri));
                string md5Sum = Util.CreateMD5(stream);
                return String.Concat(md5Sum, ext);
            });
    }
}
