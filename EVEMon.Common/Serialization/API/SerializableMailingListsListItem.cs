using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableMailingListsListItem
    {
        [XmlAttribute("listID")]
        public long ListID
        {
            get;
            set;
        }

        [XmlAttribute("displayName")]
        public string ListName
        {
            get;
            set;
        }
    }
}
