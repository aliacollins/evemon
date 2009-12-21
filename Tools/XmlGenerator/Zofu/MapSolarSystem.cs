using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.XmlImporter.Zofu;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class MapSolarSystem : IHasID
    {
        [XmlElement("solarSystemID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("solarSystemName")]
        public string Name;

        [XmlElement("security")]
        public float SecurityLevel;

        [XmlElement("constellationID")]
        public int ConstellationID;

        [XmlElement("x")]
        public double X;

        [XmlElement("y")]
        public double Y;

        [XmlElement("z")]
        public double Z;

    }
}
