using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a prerequisite material for a blueprint.
    /// </summary>
    public sealed class SerializableRequiredMaterial
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
        public long Quantity { get; set; }

        /// <summary>
        /// Gets or sets the damage per job.
        /// </summary>
        /// <value>The damage per job.</value>
        [XmlAttribute("damagePerJob")]
        public double DamagePerJob { get; set; }

        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        [XmlAttribute("activityId")]
        public int Activity { get; set; }

        /// <summary>
        /// Gets or sets the waste affected.
        /// </summary>
        /// <value>The waste affected.</value>
        [XmlAttribute("wasted")]
        public int WasteAffected { get; set; }
    }
}