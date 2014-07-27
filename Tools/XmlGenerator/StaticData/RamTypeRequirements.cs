using System.Xml.Serialization;

namespace EVEMon.XmlGenerator.StaticData
{
    public sealed class RamTypeRequirements : IHasID
    {
        [XmlElement("typeID")]
        public int ID { get; set; }

        [XmlElement("activityID")]
        public int ActivityID { get; set; }

        [XmlElement("requiredTypeID")]
        public int RequiredTypeID { get; set; }

        [XmlElement("quantity")]
        public int? Quantity { get; set; }

        [XmlElement("damagePerJob")]
        public double? DamagePerJob { get; set; }

        [XmlElement("recycle")]
        public bool? Recyclable { get; set; }

        [XmlElement("level")]
        public int? Level { get; set; }
    }
}