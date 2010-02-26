using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using EVEMon.Common.Data;

namespace EVEMon.Common.Serialization.API
{
    /// <summary>
    /// Represents a serializable version of the conquerable stations list. Used for querying CCP.
    /// </summary>
    public sealed class SerializableConquerableStationList
    {
        public SerializableConquerableStationList()
        {
            this.Outposts = new List<SerializableOutpost>();
        }

        [XmlArray("outposts")]
        [XmlArrayItem("outpost")]
        public List<SerializableOutpost> Outposts
        {
            get;
            set;
        }
    }

    public sealed class SerializableOutpost
    {
        [XmlAttribute("stationID")]
        public int StationID
        {
            get;
            set;
        }

        [XmlAttribute("stationName")]
        public string StationName
        {
            get;
            set;
        }

        [XmlAttribute("stationTypeID")]
        public int StationTypeID
        {
            get;
            set;
        }

        [XmlAttribute("solarSystemID")]
        public int SolarSystemID
        {
            get;
            set;
        }

        [XmlAttribute("corporationID")]
        public int CorporationID
        {
            get;
            set;
        }

        [XmlAttribute("corporationName")]
        public string CorporationName
        {
            get;
            set;
        }
    }
}
