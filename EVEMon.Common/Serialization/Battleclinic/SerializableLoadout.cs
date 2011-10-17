using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
{
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableLoadout
    {
        private string m_loadoutName;
        private readonly Collection<SerializableLoadoutSlot> m_slots;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableLoadout"/> class.
        /// </summary>
        public SerializableLoadout()
        {
            m_slots = new Collection<SerializableLoadoutSlot>();
        }

        /// <summary>
        /// Gets or sets the name of the loadout.
        /// </summary>
        /// <value>The name of the loadout.</value>
        [XmlAttribute("name")]
        public string LoadoutName
        {
            get { return m_loadoutName.HtmlDecode(); }
            set { m_loadoutName = value; }
        }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [XmlAttribute("Author")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        /// <value>The rating.</value>
        [XmlAttribute("rating")]
        public double Rating { get; set; }

        /// <summary>
        /// Gets or sets the loadout id.
        /// </summary>
        /// <value>The loadout id.</value>
        [XmlAttribute("loadoutID")]
        public string LoadoutID { get; set; }

        /// <summary>
        /// Gets or sets the submission date string.
        /// </summary>
        /// <value>The submission date string.</value>
        [XmlAttribute("date")]
        public string SubmissionDateXml { get; set; }

        /// <summary>
        /// Gets the submission date.
        /// </summary>
        /// <value>The submission date.</value>
        [XmlIgnore]
        public DateTime SubmissionDate
        {
            get
            {
                DateTime parsedDate;
                return DateTime.TryParse(SubmissionDateXml, out parsedDate) ? parsedDate : DateTime.MinValue;
            }
        }

        /// <summary>
        /// Gets or sets the topic.
        /// </summary>
        /// <value>The topic.</value>
        [XmlAttribute("topic")]
        public int Topic { get; set; }

        /// <summary>
        /// Gets the slots.
        /// </summary>
        /// <value>The slots.</value>
        [XmlElement("slot")]
        public Collection<SerializableLoadoutSlot> Slots { get { return m_slots; } }
    }
}