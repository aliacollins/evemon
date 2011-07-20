using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    public sealed class SerializableItemMaterials
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlElement("material")]
        public SerializableMaterialQuantity[] Materials
        {
            get;
            set;
        }
    }
}
