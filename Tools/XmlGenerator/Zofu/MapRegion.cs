using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.XmlImporter.Zofu;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class MapRegion : IHasID
    {
        [XmlElement("regionID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("regionName")]
        public string Name;

        [XmlElement("factionID")]
        public Nullable<int> FactionID;
    }
}
