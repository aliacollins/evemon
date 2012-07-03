using System;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.API
{
    public sealed class SerializableContactListItem
    {
        [XmlAttribute("contactID")]
        public long ContactID { get; set; }

        [XmlAttribute("contactName")]
        public string ContactName { get; set; }

        [XmlAttribute("inWatchlist")]
        public string InWatchlistXml
        {
            get { return InWatchlist.ToString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    InWatchlist = Convert.ToBoolean(value);
            }
        }

        [XmlAttribute("standing")]
        public float Standing { get; set; }

        [XmlIgnore]
        public ContactGroup Group { get; set; }

        [XmlIgnore]
        public bool InWatchlist { get; set; }
    }
}