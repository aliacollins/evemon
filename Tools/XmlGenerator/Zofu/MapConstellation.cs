using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.XmlImporter.Zofu;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class MapConstellation : IHasID
    {
        [XmlElement("constellationID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("constellationName")]
        public string Name;

        [XmlElement("regionID")]
        public int RegionID;
    }
}
