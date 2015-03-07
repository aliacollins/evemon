using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic.Loadout
{
    public sealed class SerializableLoadoutSlot
    {
        /// <summary>
        /// Gets or sets the type of the slot.
        /// </summary>
        /// <value>The type of the slot.</value>
        [XmlAttribute("type")]
        public string SlotType { get; set; }

        /// <summary>
        /// Gets or sets the slot position.
        /// </summary>
        /// <value>The slot position.</value>
        [XmlAttribute("position")]
        public string SlotPosition { get; set; }

        /// <summary>
        /// Gets or sets the item ID.
        /// </summary>
        /// <value>The item ID.</value>
        [XmlText]
        public int ItemID { get; set; }
    }
}