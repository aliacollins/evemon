using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using EVEMon.Common.Net;
using EVEMon.Common.Threading;

namespace EVEMon.Common
{
    public class ImageService
    {
        private static readonly Object s_syncLock = new object();

        /// <summary>
        /// Gets the directory used to store cached images but (not portraits).
        /// </summary>
        private static string ImageCacheDirectory
        {
            get
            {
                string cacheDir = Path.Combine(Path.Combine(EveClient.EVEMonDataDir, "cache"), "images");
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }
                return cacheDir;
            }
        }

        /// <summary>
        /// Asynchronously downloads a character portrait from its ID.
        /// </summary>
        /// <param name="charId"></param>
        /// <param name="callback">Callback that will be invoked on the UI thread.</param>
        public static void GetCharacterImageAsync(long charId, GetImageCallback callback)
        {
            GetImageAsync(String.Format(NetworkConstants.CCPPortraits, charId.ToString()), false, callback);
        }

        /// <summary>
        /// Asynchronously downloads an image from the provided url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="useCache"></param>
        /// <param name="callback">Callback that will be invoked on the UI thread.</param>
        public static void GetImageAsync(string url, bool useCache, GetImageCallback callback)
        {
            // Cache not to be used ?
            if (!useCache)
            {
                EveClient.HttpWebService.DownloadImageAsync(url, GotImage, callback);
                return;
            }

            // First check whether the image exists in cache.
            string cacheFileName = Path.Combine(ImageCacheDirectory, GetCacheName(url));
            if (File.Exists(cacheFileName))
            {
                try
                {
                    Image img = Image.FromFile(cacheFileName, true); 
                    callback(img);
                    return;
                }
                catch (Exception e)
                {
                    ExceptionHandler.LogException(e, false);
                }
            }

            // In last resort, downloads it.
            EveClient.HttpWebService.DownloadImageAsync(url, GotImage, (GetImageCallback)((img) =>
                {
                    if (img != null)
                    {
                        AddImageToCache(url, img);
                    }
                    callback(img);
                }));
        }

        /// <summary>
        /// Adds the image to the memory cache, flush the cache to the hard drive, then save the image to a cached file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="i"></param>
        private static void AddImageToCache(string url, Image i)
        {
            lock (s_syncLock)
            {
                string cacheName = GetCacheName(url);


                // Make sure the file map is writeable, or attemp to make it so by prompting the user. Returns on denial.
                string mapFileName = Path.Combine(ImageCacheDirectory, "file.map");
                if (File.Exists(mapFileName))
                {
                    if (!FileHelper.TryMakeWritable(mapFileName)) return;
                }

                // Appends this entry to the map file
                using (StreamWriter sw = new StreamWriter(mapFileName, true))
                {
                    sw.WriteLine(String.Format("{0} {1}", cacheName, url));
                    sw.Close();
                }

                // Saves the image file
                try
                {
                    // Write this image to a temp file name
                    string tempFileName = Path.GetTempFileName();
                    using (FileStream fs = new FileStream(tempFileName, FileMode.Create))
                    {
                        i.Save(fs, ImageFormat.Png);
                        fs.Flush();
                    }

                    // Move to the file
                    string fn = Path.Combine(ImageCacheDirectory, cacheName);
                    FileHelper.OverwriteOrWarnTheUser(tempFileName, fn, OverwriteOperation.Move);
                }
                catch (Exception e)
                {
                    ExceptionHandler.LogException(e, false);
                }
            }
        }

        /// <summary>
        /// From a given url, computes a cache file name.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetCacheName(string url)
        {
            Match extensionMatch = Regex.Match(url, @"([^\.]+)$");
            string ext = String.Empty;
            if (extensionMatch.Success)
            {
                ext = "." + extensionMatch.Groups[1];
            }
            byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(url));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(String.Format("{0:x2}", hash[i]));
            }
            sb.Append(ext);
            return sb.ToString();
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
                // Invokes on the UI thread.
                Dispatcher.BeginInvoke(() => callback(e.Result));
            }
            else
            {
                ExceptionHandler.LogException(e.Error, true);
                callback(null);
            }
        }
    }

    public delegate void GetImageCallback(Image i);
}
