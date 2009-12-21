using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.XmlImporter.Zofu;
using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.Zofu
{
    public sealed class InvType : IHasID
    {
        [XmlElement("typeID")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("groupID")]
        public int GroupID;

        [XmlElement("graphicID")]
        public Nullable<int> GraphicID;

        [XmlElement("typeName")]
        public string Name;

        [XmlElement("description")]
        public string Description;

        [XmlElement("mass")]
        public double Mass;

        [XmlElement("volume")]
        public double Volume;

        [XmlElement("capacity")]
        public double Capacity;

        [XmlElement("raceID")]
        public Nullable<int> RaceID;

        [XmlElement("marketGroupID")]
        public Nullable<int> MarketGroupID;

        [XmlElement("basePrice")]
        public decimal BasePrice;

        [XmlElement("published")]
        public int Published;

        [XmlIgnore]
        public bool Generated;
    }
}
