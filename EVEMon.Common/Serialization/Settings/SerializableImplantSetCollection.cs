using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a collection of implants sets.
    /// </summary>
    public sealed class SerializableImplantSetCollection
    {
        private readonly Collection<SerializableSettingsImplantSet> m_customSets;

        public SerializableImplantSetCollection()
        {
            API = new SerializableSettingsImplantSet();
            OldAPI = new SerializableSettingsImplantSet();
            m_customSets = new Collection<SerializableSettingsImplantSet>();
        }

        [XmlElement("api")]
        public SerializableSettingsImplantSet API { get; set; }

        [XmlElement("oldApi")]
        public SerializableSettingsImplantSet OldAPI { get; set; }

        [XmlElement("customSet")]
        public Collection<SerializableSettingsImplantSet> CustomSets
        {
            get { return m_customSets; }
        }

        [XmlElement("selectedIndex")]
        public int SelectedIndex { get; set; }
    }
}