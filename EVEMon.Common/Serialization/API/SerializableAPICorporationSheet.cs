using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

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
        public string NameXml
        {
            get { return Name; }
            set { Name = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlElement("ticker")]
        public string Ticker { get; set; }

        [XmlElement("ceoID")]
        public long CeoID { get; set; }

        [XmlElement("ceoName")]
        public string CeoNameXml
        {
            get { return CeoName; }
            set { CeoName = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlElement("stationID")]
        public long HQStationID { get; set; }

        [XmlElement("stationName")]
        public string HQStationNameXml
        {
            get { return HQStationName; }
            set { HQStationName = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlElement("description")]
        public string DescriptionXml
        {
            get { return Description; }
            set { Description = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlElement("url")]
        public string WebUrl { get; set; }

        [XmlElement("allianceID")]
        public long AllianceID { get; set; }

        [XmlElement("allianceName")]
        public string AllianceNameXml
        {
            get { return AllianceName; }
            set { AllianceName = value == null ? String.Empty : value.HtmlDecode(); }
        }

        [XmlElement("factionID")]
        public int FactionID { get; set; }

        [XmlElement("factionName")]
        public string FactionName { get; set; }

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

        [XmlIgnore]
        public string Name { get; set; }

        [XmlIgnore]
        public string CeoName { get; set; }

        [XmlIgnore]
        public string HQStationName { get; set; }

        [XmlIgnore]
        public string AllianceName { get; set; }

        [XmlIgnore]
        public string Description { get; set; }
    }
}
