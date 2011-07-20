using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Root ItemsDatafile Serialization Class
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
