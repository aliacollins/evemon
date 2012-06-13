using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.FittingXml
{
    public sealed class SerializableFitting
    {
        private readonly Collection<SerializableFittingHardware> m_fittingHardwares;

        public SerializableFitting()
        {
            m_fittingHardwares = new Collection<SerializableFittingHardware>();
        }

        [XmlElement("hardware")]
        public Collection<SerializableFittingHardware> FittingHardware
        {
            get { return m_fittingHardwares; }
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("description")]
        public SerializableFittingDescription Description { get; set; }

        [XmlElement("shipType")]
        public SerializableFittingShipType ShipType { get; set; }
    }
}