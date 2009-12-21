using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace EVEMon.Common
{
    /// <summary>
    /// Represents the cache for the XML character sheet downloaded from CCP. 
    /// </summary>
    public static class LocalXmlCache
    {
        private static readonly object m_syncLock = new object();
        private static string m_cacheDirectory;


        /// <summary>
        /// Static constructor.
        /// </summary>
        public static void Initialize()
        {
            string EVEMonDir = Path.Combine(EveClient.EVEMonDataDir, "cache");

            if (!Directory.Exists(EVEMonDir)) Directory.CreateDirectory(EVEMonDir);

            EVEMonDir = Path.Combine(EVEMonDir, "xml");
            if (!Directory.Exists(EVEMonDir)) Directory.CreateDirectory(EVEMonDir);

            m_cacheDirectory = EVEMonDir + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Gets the <see cref="System.IO.FileInfo"/> for the specified character xml.
        /// If you really want the xml, use GetCharacterXml
        /// </summary>
        /// <value></value>
        public static FileInfo GetFile(string charName)
        {
            lock (m_syncLock)
            {
                string encodedName = charName + ".xml";
                return new FileInfo(m_cacheDirectory + encodedName);
            }
        }

        /// <summary>
        /// Gets the character XML
        /// </summary>
        /// <param name="charName"></param>
        /// <returns></returns>
        public static XmlDocument GetCharacterXml(string charName)
        {
            lock (m_syncLock)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(m_cacheDirectory + charName + ".xml");
                return doc;
            }
        }

        /// <summary>
        /// The preferred way to save - this should be a <see cref="System.Xml.XmlDocument"/> straight from CCP
        /// </summary>
        /// <param name="xdoc">The character xml to save.</param>
        internal static void Save(string key, XmlDocument xdoc)
        {
            lock (m_syncLock)
            {
                XmlNode characterNode = xdoc.SelectSingleNode("//name");
                string name = characterNode.InnerText;
                string encodedName = name + ".xml";

                // Writes in a temporary file name
                string tempFileName = Path.GetTempFileName();
                using (XmlTextWriter writer = new XmlTextWriter(new FileStream(tempFileName, FileMode.Create), Encoding.GetEncoding("iso-8859-1")))
                {
                    xdoc.WriteTo(writer);
                    writer.Flush();
                    writer.Close();
                }

                // Overwrite the target file
                string fileName = Path.Combine(m_cacheDirectory, characterNode.InnerText + ".xml");
                FileHelper.OverwriteOrWarnTheUser(tempFileName, fileName, OverwriteOperation.Move);
            }
        }

        /// <summary>
        /// Gets the URI for the cached xml for the given character
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        internal static Uri GetCharacterUri(string characterName)
        {
            lock (m_syncLock)
            {
                return new Uri(m_cacheDirectory + characterName + ".xml");
            }
        }
    }
}
