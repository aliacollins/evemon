using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace EVEMon.Common
{
    public sealed class LocalXmlCache
    {
        static WeakReference<LocalXmlCache> instance;
        private string _cacheDirectory;


        /// <summary>
        /// Initializes the <see cref="LocalXmlCache"/> class.
        /// This constructor exists purely to ensure lazy initialization
        /// </summary>
        static LocalXmlCache()
        {
            instance = new WeakReference<LocalXmlCache>(new LocalXmlCache());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalXmlCache"/> class.
        /// </summary>
        LocalXmlCache()
        {
            string evemonDir = Path.Combine(Settings.EveMonDataDir, "cache");

            if (!Directory.Exists(evemonDir))
                Directory.CreateDirectory(evemonDir);

            evemonDir = Path.Combine(evemonDir,  "xml");
            if (!Directory.Exists(evemonDir))
                Directory.CreateDirectory(evemonDir);

            _cacheDirectory = evemonDir + Path.DirectorySeparatorChar;
        }


        /// <summary>
        /// Gets the singleton instance of the LocalXmlCache object
        /// </summary>
        /// <value>The singleton instance - no more than one of these, ever.</value>
        public static LocalXmlCache Instance
        {
            get
            {
                LocalXmlCache targ = null;
                if (instance != null)
                {
                    targ = instance.Target;
                }
                if (targ == null)
                {
                    targ = new LocalXmlCache();
                    instance = new WeakReference<LocalXmlCache>(targ);
                }
                return targ;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.IO.FileInfo"/> for the specified character xml.
        /// If you really want the xml, use GetCharacterXml
        /// </summary>
        /// <value></value>
        public FileInfo this[string charName]
        {
            get
            {
                string encodedName = charName + ".xml";
                return new FileInfo(_cacheDirectory + encodedName);
            }
        }

        public XmlDocument GetCharacterXml(string charName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_cacheDirectory + charName + ".xml");
            return doc;
        }

        /// <summary>
        /// The preferred way to save - this should be a <see cref="System.Xml.XmlDocument"/> straight from CCP
        /// </summary>
        /// <param name="xdoc">The character xml to save.</param>
        public void Save(XmlDocument xdoc)
        {
            XmlNode characterNode = xdoc.SelectSingleNode("//name");
            string name = characterNode.InnerText;
            string encodedName = name+".xml";

            // Writes in a temporary file name
            string tempFileName = Path.GetTempFileName();
            using (XmlTextWriter writer = new XmlTextWriter(new FileStream(tempFileName, FileMode.Create), Encoding.GetEncoding("iso-8859-1")))
            {
                xdoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
            }

            // Overwrite the target file
            string fileName = Path.Combine(_cacheDirectory, characterNode.InnerText + ".xml");
            LocalFileSystem.OverwriteOrWarnTheUser(tempFileName, fileName, OverwriteOperation.Move);
        }
    }
}
