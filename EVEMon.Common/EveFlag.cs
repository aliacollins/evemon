using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using EVEMon.Common.Serialization;

namespace EVEMon.Common
{
    public static class EveFlag
    {
        private static SerializableEveFlags s_eveFlags;
        private static bool s_isLoaded;

        /// <summary>
        /// Gets the description of the flag.
        /// </summary>
        /// <param name="id">The flag id.</param>
        /// <returns></returns>
        internal static string GetFlagText(int id)
        {
            EnsureInitialized();

            SerializableEveFlagsListItem flag = s_eveFlags.EVEFlags.FirstOrDefault(x => x.ID == id);
            return flag != null ? flag.Text : "Unknown";
        }

        /// <summary>
        /// Ensures the notification types data have been intialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (s_isLoaded)
                return;

            // Read the resource file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Properties.Resources.Flags);

            // Read the nodes
            using (XmlNodeReader reader = new XmlNodeReader(xmlDoc))
            {
                // Create a memory stream to transform the xml 
                MemoryStream stream = Util.GetMemoryStream();

                // Write the xml output to the stream
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    // Apply the XSL transform
                    writer.Formatting = Formatting.Indented;
                    APIProvider.RowsetsTransform.Transform(reader, writer);
                    writer.Flush();

                    // Deserialize from the given stream
                    stream.Seek(0, SeekOrigin.Begin);
                    XmlSerializer xs = new XmlSerializer(typeof(SerializableEveFlags));
                    s_eveFlags = (SerializableEveFlags)xs.Deserialize(stream);
                }
            }

            s_isLoaded = true;
        }
    }
}
