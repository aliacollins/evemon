using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using EVEMon.Common.Extensions;
using EVEMon.Common.Helpers;
using EVEMon.Common.Models;

namespace EVEMon.Common.Service
{
    /// <summary>
    /// Represents the cache for the XML files downloaded from CCP. 
    /// </summary>
    public static class LocalXmlCache
    {
        private static readonly object s_syncLock = new object();

        /// <summary>
        /// Gets the <see cref="System.IO.FileInfo" /> for the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static FileInfo GetFileInfo(string filename)
        {
            lock (s_syncLock)
            {
                EveMonClient.EnsureCacheDirInit();

                return new FileInfo(Path.Combine(EveMonClient.EVEMonXmlCacheDir, $"{filename}.xml"));
            }
        }

        /// <summary>
        /// Gets the character XML.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IXPathNavigable GetCharacterXml(Character character)
        {
            character.ThrowIfNull(nameof(character));

            EveMonClient.EnsureCacheDirInit();

            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(EveMonClient.EVEMonXmlCacheDir, $"{character.Name}.xml"));
            return doc;
        }

        /// <summary>
        /// Gets the URI for the cached xml for the given character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        [Obsolete]
        internal static Uri GetCharacterUri(Character character)
        {
            character.ThrowIfNull(nameof(character));

            EveMonClient.EnsureCacheDirInit();

            return new Uri(Path.Combine(EveMonClient.EVEMonXmlCacheDir, $"{character.Name}.xml"));
        }

        /// <summary>
        /// The preferred way to save - this should be a <see cref="System.Xml.XmlDocument" /> straight from CCP.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="xdoc">The xml to save.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static async Task SaveAsync(string filename, IXPathNavigable xdoc)
        {
            xdoc.ThrowIfNull(nameof(xdoc));

            EveMonClient.EnsureCacheDirInit();

            XmlDocument xmlDoc = (XmlDocument)xdoc;
            XmlNode characterNode = xmlDoc.SelectSingleNode("//name");
            filename = characterNode?.InnerText ?? filename;

            // Writes in the target file
            string fileName = Path.Combine(EveMonClient.EVEMonXmlCacheDir, $"{filename}.xml");
            string content = Util.GetXmlStringRepresentation(xdoc);
            await FileHelper.OverwriteOrWarnTheUserAsync(fileName,
                async fs =>
                {
                    using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        await writer.WriteAsync(content);
                        await writer.FlushAsync();
                        await fs.FlushAsync();
                    }
                    return true;
                });
        }

        /// <summary>
        /// Deletes the given filename.
        /// </summary>
        ///
        /// <param name="filename"> The filename. </param>
        public static void Delete(string filename)
        {
            EveMonClient.EnsureCacheDirInit();

            // Writes in the target file
            string fileName = Path.Combine(EveMonClient.EVEMonXmlCacheDir, $"{filename}.xml");
            FileHelper.DeleteFile(fileName);
        }

        /// <summary>
        /// Check if file is downloaded and up to date.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="updateTime">The update time.</param>
        /// <param name="period">The period.</param>
        /// <returns>
        /// True if file is up to date, False otherwise
        /// </returns>
        internal static bool CheckFileUpToDate(string filename, DateTime updateTime, TimeSpan period)
        {
            FileInfo file = GetFileInfo(filename);
            DateTime previousUpdateTime = updateTime.Subtract(period);

            // File is already downloaded ?
            if (!File.Exists(file.FullName))
                return false;

            // Is it up to date ?
            // (file is updated after the update time
            // or was updated between the previous day update time
            // and today's update time and its not time to update yet) ?
            return file.LastWriteTimeUtc > updateTime
                   || (file.LastWriteTimeUtc > previousUpdateTime &&
                       file.LastWriteTimeUtc < updateTime &&
                       DateTime.UtcNow < updateTime);
        }
    }
}
