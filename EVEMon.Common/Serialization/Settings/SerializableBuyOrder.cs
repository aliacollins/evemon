using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a buy order.
    /// </summary>
    public sealed class SerializableBuyOrder : SerializableOrderBase
    {
        /// <summary>
        /// How much ISK is in escrow.
        /// </summary>
        [XmlAttribute("escrow")]
        public decimal Escrow { get; set; }

        /// <summary>
        /// -1 = station, 0 = solar system, 1 = 1 jump, 2 = 2 jumps, ..., 32767 = region.
        /// </summary>
        [XmlAttribute("range")]
        public int Range { get; set; }
    }
}