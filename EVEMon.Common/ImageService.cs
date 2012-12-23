using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EVEMon.Common.Net;
using EVEMon.Common.Threading;

namespace EVEMon.Common
{
    public delegate void GetImageCallback(Image i);

    public static class ImageService
    {
        private static readonly Object s_syncLock = new object();

        /// <summary>
        /// Asynchronously downloads a character portrait from its ID.
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="callback">Callback that will be invoked on the UI thread.</param>
        public static void GetCharacterImageAsync(long charId, GetImageCallback callback)
        {
            GetImageAsync(new Uri(String.Format(CultureConstants.InvariantCulture,
                                                NetworkConstants.CCPPortraits, charId, (int)EveImageSize.x128)), callback, false);
        }

        /// <summary>
        /// Asynchronously downloads an alliance image.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        /// <param name="allianceID">The alliance ID.</param>
        public static void GetAllianceImage(PictureBox pictureBox, long allianceID)
        {
            Uri url = new Uri(String.Format(CultureConstants.InvariantCulture, NetworkConstants.CCPIconsFromImageServer,
                                            "alliance", allianceID, pictureBox.Width));

            GetImageAsync(url, (img =>
                                    {
                                        pictureBox.Image = img ?? pictureBox.InitialImage;
                                        pictureBox.Update();
                                    }));
        }

        /// <summary>
        /// Asynchronously downloads a corporation image.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        /// <param name="corporationID">The corporation ID.</param>
        public static void GetCorporationImage(PictureBox pictureBox, long corporationID)
        {
            Uri url = new Uri(String.Format(CultureConstants.InvariantCulture, NetworkConstants.CCPIconsFromImageServer,
                                            "corporation", corporationID, pictureBox.Width));

            GetImageAsync(url, (img =>
                                    {
                                        pictureBox.Image = img ?? pictureBox.InitialImage;
                                        pictureBox.Update();
                                    }));
        }

        /// <summary>
        /// Asynchronously downloads an image from the provided url.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="useCache">if set to <c>true</c> [use cache].</param>
        /// <param name="callback">Callback that will be invoked on the UI thread.</param>
        public static void GetImageAsync(Uri url, GetImageCallback callback, bool useCache = true)
        {
            // Cache not to be used ?
            if (!useCache)
            {
                HttpWebService.DownloadImageAsync(url, GotImage, callback);
                return;
            }

            // First check whether the image exists in cache
            EveMonClient.EnsureCacheDirInit();
            string cacheFileName = Path.Combine(EveMonClient.EVEMonImageCacheDir, GetCacheName(url));
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
                    callback(image);
                    return;
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
            HttpWebService.DownloadImageAsync(url, GotImage,
                                              (GetImageCallback)(img =>
                                                                     {
                                                                         if (img != null)
                                                                             AddImageToCache(url, img);

                                                                         callback(img);
                                                                     }));
        }

        /// <summary>
        /// Adds the image to the memory cache, flush the cache to the hard drive, then save the image to a cached file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="image"></param>
        private static void AddImageToCache(Uri url, Image image)
        {
            lock (s_syncLock)
            {
                // Saves the image file
                try
                {
                    // Write this image to the cache file
                    EveMonClient.EnsureCacheDirInit();
                    string cacheFileName = Path.Combine(EveMonClient.EVEMonImageCacheDir, GetCacheName(url));
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
            }
        }

        /// <summary>
        /// From a given url, computes a cache file name.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetCacheName(Uri url)
        {
            Match extensionMatch = Regex.Match(url.AbsoluteUri, @"([^\.]+)$");
            string ext = String.Empty;
            if (extensionMatch.Success)
                ext = "." + extensionMatch.Groups[1];

            Stream stream = Util.GetMemoryStream(Encoding.UTF8.GetBytes(url.AbsoluteUri));
            string md5Sum = Util.CreateMD5(stream);
            return String.Concat(md5Sum, ext);
        }

        /// <summary>
        /// Callback used when images are downloaded, it takes care to invoke another callback provided as our user state.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="state"></param>
        private static void GotImage(DownloadImageAsyncResult e, object state)
        {
            GetImageCallback callback = (GetImageCallback)state;
            if (e.Error == null)
            {
                // Invokes on the UI thread
                Dispatcher.BeginInvoke(() => callback(e.Result));
            }
            else
            {
                if (e.Error.Status == HttpWebServiceExceptionStatus.Timeout)
                    EveMonClient.Trace("ImageService: {0}", e.Error.Message);
                else
                    ExceptionHandler.LogException(e.Error, true);

                callback(null);
            }
        }
    }
}