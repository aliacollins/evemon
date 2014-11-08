using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{     

    /// <summary>
    /// Represents a serializable version of a character's sheet. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPICharacterSheet : SerializableCharacterSheetBase
    {
        private readonly Collection<SerializableNewImplant> m_newImplants;

        public SerializableAPICharacterSheet()
        {
            m_newImplants = new Collection<SerializableNewImplant>();
        }

        [XmlElement("attributeEnhancers")]
        public SerializableImplantSet OldImplants { get; set; }

        [XmlArray("implants")]
        [XmlArrayItem("implant")]
        public Collection<SerializableNewImplant> NewImplants
        {
            get { return m_newImplants; }
        }
    }
}