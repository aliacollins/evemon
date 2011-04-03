using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using EVEMon.Common.Serialization;
using System.Text;
using System.Xml.Xsl;

namespace EVEMon.Common
{
    public static class EveNotificationType
    {
        private static SerializableNotificationRefTypeIDs s_notificationTypes;
        private static bool s_isLoaded;


        internal static string GetType(int typeID)
        {
            EnsureInitialized();

            var type = s_notificationTypes.Types.FirstOrDefault(x => x.TypeID == typeID);
            if (type != null)
                return type.TypeName;

            return "Unknown";
        }

        private static void EnsureInitialized()
        {
            if (s_isLoaded)
                return;

            // Read the resource file
            using (StringReader stringReader = new StringReader(Properties.Resources.NotificationRefTypeIDs))
            {
                // Format it as xml
                using (XmlTextReader reader = new XmlTextReader(stringReader))
                {
                    // Create a memory stream to write to 
                    using (MemoryStream stream = new MemoryStream())
                    {
                        // Write the xml output to the stream
                        using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                        {
                            // Apply the XSL transform
                            XslCompiledTransform transform = Util.LoadXSLT(Properties.Resources.RowsetsXSLT);
                            writer.Formatting = Formatting.Indented;
                            transform.Transform(reader, writer);
                            writer.Flush();

                            // Deserialize from the given stream
                            stream.Seek(0, SeekOrigin.Begin);
                            XmlSerializer xs = new XmlSerializer(typeof(SerializableNotificationRefTypeIDs));
                            s_notificationTypes = (SerializableNotificationRefTypeIDs)xs.Deserialize(stream);
                        }
                    }
                }
            }

            s_isLoaded = true;
        }

    }
}
