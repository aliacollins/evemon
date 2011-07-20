using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    public sealed class SerializableProperty
    {
        [XmlElement("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("description")]
        public string Description
        {
            get;
            set;
        }

        [XmlElement("defaultValue")]
        public string DefaultValue
        {
            get;
            set;
        }

        [XmlElement("icon")]
        public string Icon
        {
            get;
            set;
        }

        [XmlElement("unit")]
        public string Unit
        {
            get;
            set;
        }

        [XmlElement("unitID")]
        public int UnitID
        {
            get;
            set;
        }

        [XmlElement("higherIsBetter")]
        public bool HigherIsBetter
        {
            get;
            set;
        }
    }
}
