using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents our items datafile.
    /// </summary>
    [XmlRoot("itemsDatafile")]
    public sealed class ItemsDatafile
    {
        [XmlElement("marketGroup")]
        public SerializableMarketGroup[] MarketGroups
        {
            get;
            set;
        }
    }
}
