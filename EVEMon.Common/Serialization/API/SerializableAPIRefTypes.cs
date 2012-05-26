using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableAPIRefTypes
    {
        private readonly Collection<SerializableRefTypesListItem> m_refTypes;

        public SerializableAPIRefTypes()
        {
            m_refTypes = new Collection<SerializableRefTypesListItem>();
        }

        [XmlArray("refTypes")]
        [XmlArrayItem("refType")]
        public Collection<SerializableRefTypesListItem> RefTypes
        {
            get { return m_refTypes; }
        }
    }
}
