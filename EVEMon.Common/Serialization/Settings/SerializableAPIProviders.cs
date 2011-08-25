using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a serializable version of the set of API providers. Used for settings persistence.
    /// </summary>
    public sealed class SerializableAPIProviders
    {
        public SerializableAPIProviders()
        {
            CustomProviders = new List<SerializableAPIProvider>();
        }

        [XmlElement("currentProvider")]
        public string CurrentProviderName { get; set; }

        [XmlArray("customProviders")]
        [XmlArrayItem("provider")]
        public List<SerializableAPIProvider> CustomProviders { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return CurrentProviderName;
        }
    }
}
