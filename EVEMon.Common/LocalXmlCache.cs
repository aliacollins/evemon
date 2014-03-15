using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the cache for the XML files downloaded from CCP. 
    /// </summary>
    public static class LocalXmlCache
    {
        private static readonly object s_syncLock = new object();

        /// <summary>
        /// Gets the <see cref="System.IO.FileInfo"/> for the specified character XML.
        /// If you really want the xml, use GetCharacterXml
        /// </summary>
        /// <value></value>
        public static FileInfo GetFile(string filename)
        {
            lock (s_syncLock)
            {
                EveMonClient.EnsureCacheDirInit();
                return new FileInfo(Path.Combine(EveMonClient.EVEMonXmlCacheDir,
                                                 String.Format(CultureConstants.DefaultCulture, "{0}.xml", filename)));
            }
        }

        /// <summary>
        /// Gets the character XML.
        /// </summary>
        /// <param name="charName"></param>
        /// <returns></returns>
        public static IXPathNavigable GetCharacterXml(string charName)
        {
            lock (s_syncLock)
            {
                XmlDocument doc = new XmlDocument();
                EveMonClient.EnsureCacheDirInit();
                doc.Load(Path.Combine(EveMonClient.EVEMonXmlCacheDir,
                                      String.Format(CultureConstants.DefaultCulture, "{0}.xml", charName)));
                return doc;
            }
        }

        /// <summary>
        /// The preferred way to save - this should be a <see cref="System.Xml.XmlDocument"/> straight from CCP.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="xdoc">The xml to save.</param>
        public static void Save(string key, IXPathNavigable xdoc)
        {
            if (xdoc == null)
                throw new ArgumentNullException("xdoc");

            lock (s_syncLock)
            {
                XmlDocument xmlDoc = (XmlDocument)xdoc;
                XmlNode characterNode = xmlDoc.SelectSingleNode("//name");
                string name = (characterNode == null ? key : characterNode.InnerText);

                // Writes in the target file
                EveMonClient.EnsureCacheDirInit();
                string fileName = Path.Combine(EveMonClient.EVEMonXmlCacheDir,
                                               String.Format(CultureConstants.DefaultCulture, "{0}.xml", name));
                string content = Util.GetXmlStringRepresentation(xdoc);
                FileHelper.OverwriteOrWarnTheUser(fileName,
                                                  fs =>
                                                      {
                                                          using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                                                          {
                                                              writer.Write(content);
                                                              writer.Flush();
                                                              fs.Flush();
                                                          }
                                                          return true;
                                                      });
            }
        }

        /// <summary>
        /// Gets the URI for the cached xml for the given character.
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        internal static Uri GetCharacterUri(string characterName)
        {
            lock (s_syncLock)
            {
                EveMonClient.EnsureCacheDirInit();
                return new Uri(Path.Combine(EveMonClient.EVEMonXmlCacheDir,
                                            String.Format(CultureConstants.DefaultCulture, "{0}.xml", characterName)));
            }
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
            FileInfo file = GetFile(filename);
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