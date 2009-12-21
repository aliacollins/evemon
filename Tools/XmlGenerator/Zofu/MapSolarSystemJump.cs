using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class MapSolarSystemJump
    {
        [XmlElement("fromSolarSystemID")]
        public int A;

        [XmlElement("toSolarSystemID")]
        public int B;
    }
}
