using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EVEMon.Common.Serialization;

namespace EVEMon.Common
{
    public static class EveNotificationType
    {
        private static SerializableNotificationRefTypeIDs s_notificationTypes;
        private static bool s_isLoaded;

        /// <summary>
        /// Gets the description of the notification type.
        /// </summary>
        /// <param name="typeID">The type ID.</param>
        /// <returns></returns>
        internal static string GetType(int typeID)
        {
            EnsureInitialized();

            SerializableNotificationRefTypeIDsListItem type = s_notificationTypes.Types.FirstOrDefault(x => x.TypeID == typeID);
            return type != null ? type.TypeName : "Unknown";
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
            xmlDoc.LoadXml(Properties.Resources.NotificationRefTypeIDs);

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
                    XmlSerializer xs = new XmlSerializer(typeof(SerializableNotificationRefTypeIDs));
                    stream.Seek(0, SeekOrigin.Begin);
                    s_notificationTypes = (SerializableNotificationRefTypeIDs)xs.Deserialize(stream);
                }
            }

            s_isLoaded = true;
        }
    }
}