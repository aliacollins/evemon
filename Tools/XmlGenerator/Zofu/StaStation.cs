using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.XmlImporter.Zofu;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class StaStation : IHasID
    {
        [XmlElement("stationID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("stationName")]
        public string Name;

        [XmlElement("security")]
        public int SecurityLevel;

        [XmlElement("solarSystemID")]
        public int SolarSystemID;

        [XmlElement("reprocessingEfficiency")]
        public float ReprocessingEfficiency;

        [XmlElement("reprocessingStationsTake")]
        public float ReprocessingStationsTake;
    }
}
