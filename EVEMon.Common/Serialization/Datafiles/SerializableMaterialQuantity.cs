using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    public sealed class SerializableMaterialQuantity
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("quantity")]
        public int Quantity
        {
            get;
            set;
        }
    }
}
