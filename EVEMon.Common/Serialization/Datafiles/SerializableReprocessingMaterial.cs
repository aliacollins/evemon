using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a reprocessing material for ships, items and implants.
    /// </summary>
    public sealed class SerializableReprocessingMaterial
    {
        [XmlAttribute("id")]
        public int ID
        {
            get;
            set;
        }

        [XmlAttribute("quantity")]
        public string Quantity
        {
            get;
            set;
        }
    }
}
