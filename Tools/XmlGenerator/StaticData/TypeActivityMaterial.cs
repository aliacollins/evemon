using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class TypeActivityMaterial
    {
        [XmlElement("typeID")]
        public int TypeID
        {
            get;
            set;
        }

        [XmlElement("requiredTypeID")]
        public int RequiredTypeID
        {
            get;
            set;
        }

        [XmlElement("activityID")]
        public int ActivityID
        {
            get;
            set;
        }

        [XmlElement("quantity")]
        public int Quantity
        {
            get;
            set;
        }
    }
}
