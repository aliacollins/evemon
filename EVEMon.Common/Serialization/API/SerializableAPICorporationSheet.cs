using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of a corporation's sheet. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPICorporationSheet
    {
        private readonly Collection<SerializableDivision> m_divisions;
        private readonly Collection<SerializableWalletDivision> m_walletDivisions;

        public SerializableAPICorporationSheet()
        {
            m_divisions = new Collection<SerializableDivision>();
            m_walletDivisions = new Collection<SerializableWalletDivision>();
        }

        [XmlElement("corporationID")]
        public long ID { get; set; }

        [XmlElement("corporationName")]
        public string Name { get; set; }

        [XmlElement("ticker")]
        public string Ticker { get; set; }

        [XmlElement("ceoID")]
        public long CeoID { get; set; }

        [XmlElement("ceoName")]
        public string CeoName { get; set; }

        [XmlElement("stationID")]
        public long HQStationID { get; set; }

        [XmlElement("stationName")]
        public string HQStationName { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("url")]
        public string WebUrl { get; set; }

        [XmlElement("allianceID")]
        public long AllianceID { get; set; }

        [XmlElement("allianceName")]
        public string AllianceName { get; set; }

        [XmlElement("taxRate")]
        public float TaxRate { get; set; }

        [XmlElement("memberCount")]
        public int MemberCount { get; set; }

        [XmlElement("memberLimit")]
        public int MemberLimit { get; set; }

        [XmlElement("shares")]
        public int Shares { get; set; }

        [XmlArray("divisions")]
        [XmlArrayItem("division")]
        public Collection<SerializableDivision> Divisions
        {
            get { return m_divisions; }
        }

        [XmlArray("walletDivisions")]
        [XmlArrayItem("walletDivision")]
        public Collection<SerializableWalletDivision> WalletDivisions
        {
            get { return m_walletDivisions; }
        }

    }
}
