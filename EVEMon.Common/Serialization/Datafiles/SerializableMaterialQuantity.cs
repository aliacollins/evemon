using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    public sealed class SerializableMaterialQuantity
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlAttribute("id")]
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        [XmlAttribute("quantity")]
        public int Quantity { get; set; }
    }
}