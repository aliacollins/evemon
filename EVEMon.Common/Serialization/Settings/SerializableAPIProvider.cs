using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a serializable version of an API provider. Used for settings persistence.
    /// </summary>
    public sealed class SerializableAPIProvider
    {
        private readonly Collection<SerializableAPIMethod> m_methods;

        public SerializableAPIProvider()
        {
            Name = "New provider";
            Address = NetworkConstants.APIBase;
            m_methods = new Collection<SerializableAPIMethod>();
        }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("url")]
        public string Address { get; set; }

        [XmlArray("methods")]
        [XmlArrayItem("method")]
        public Collection<SerializableAPIMethod> Methods
        {
            get { return m_methods; }
        }
    }
}