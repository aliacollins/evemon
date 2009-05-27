using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using EVEMon.Common.Net;

namespace EVEMon.Common
{
    public class ImageService
    {
        public static void GetCharacterImageAsync(int charId, GetImageCallback callback)
        {
            GetImageAsync("http://img.eve.is/serv.asp?s=256&c=" + charId, false, callback);
        }

        private static string ImageCacheDirectory
        {
            get
            {
                string cacheDir = String.Format("{1}{0}cache{0}images", Path.DirectorySeparatorChar, Settings.EveMonDataDir);
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }
                return cacheDir;
            }
        }

        public static void GetImageAsync(string url, bool useCache, GetImageCallback callback)
        {
            if (useCache)
            {
                string cacheFileName = Path.Combine(ImageCacheDirectory, GetCacheName(url));
                if (File.Exists(cacheFileName))
                {
                    try
                    {
                        Image i = Image.FromFile(cacheFileName, true); 
                        callback(null, i);
                        return;
                    }
                    catch (Exception e)
                    {
                        ExceptionHandler.LogException(e, false);
                    }
                }
                GetImageCallback origCallback = callback;
                callback = new GetImageCallback(
                    delegate(EveSession s, Image i)
                        {
                            if (i != null)
                            {
                                AddImageToCache(url, i);
                            }
                            origCallback(s, i);
                        }
                    );
            }
            CommonContext.HttpWebService.DownloadImageAsync(url, GotImage, new AsyncImageRequestState(callback));
        }

        private class AsyncImageRequestState
        {
            private readonly GetImageCallback _callback;

            public AsyncImageRequestState(GetImageCallback callback)
            {
                _callback = callback;
            }

            public GetImageCallback Callback
            {
                get { return _callback; }
            }
        }

        private static void GotImage(DownloadImageAsyncResult e, object state)
        {
            AsyncImageRequestState requestState = (AsyncImageRequestState) state;
            if (e.Error == null)
            {
                requestState.Callback(null, e.Result);
            }
            else
            {
                ExceptionHandler.LogException(e.Error, true);
                requestState.Callback(null, null);
            }
        }

        /// <summary>
        /// Adds the image to the memory cache, flush the cache to the hard drive, then save the image to a cached file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="i"></param>
        private static void AddImageToCache(string url, Image i)
        {
            string cacheName = GetCacheName(url);


            // Make sure the file map is writeable, or attemp to make it so by prompting the user. Returns on denial.
            string mapFileName = Path.Combine(ImageCacheDirectory, "file.map");
            if (File.Exists(mapFileName))
            {
                if (!LocalFileSystem.TryMakeWritable(mapFileName)) return;
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
                LocalFileSystem.OverwriteOrWarnTheUser(tempFileName, fn, OverwriteOperation.Move);
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, false);
            }
        }

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
    }
}
