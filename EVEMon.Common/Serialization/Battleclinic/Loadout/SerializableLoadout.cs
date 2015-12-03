using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.BattleClinic.Loadout
{
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class SerializableLoadout
    {
        private readonly Collection<SerializableLoadoutSlot> m_slots;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableLoadout"/> class.
        /// </summary>
        public SerializableLoadout()
        {
            m_slots = new Collection<SerializableLoadoutSlot>();
        }

        /// <summary>
        /// Gets or sets the name of the loadout from the xml.
        /// </summary>
        /// <value>The name of the loadout.</value>
        [XmlAttribute("name")]
        public string NameXml
        {
            get { return Name; }
            set { Name = value == null ? String.Empty : value.HtmlDecode(); }
        }

        /// <summary>
        /// Gets or sets the name of the loadout.
        /// </summary>
        /// <value>The name of the loadout.</value>
        [XmlIgnore]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the author from the xml.
        /// </summary>
        /// <value>The author.</value>
        [XmlAttribute("Author")]
        public string AuthorXml
        {
            get { return Author; }
            set { Author = value == null ? String.Empty : value.HtmlDecode(); }
        }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [XmlIgnore]
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
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the submission date string.
        /// </summary>
        /// <value>The submission date string.</value>
        [XmlAttribute("date")]
        public string SubmissionDateXml
        {
            get { return SubmissionDate.DateTimeToTimeString(); }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    SubmissionDate = value.TimeStringToDateTime();
            }
        }

        /// <summary>
        /// Gets the submission date.
        /// </summary>
        /// <value>The submission date.</value>
        [XmlIgnore]
        public DateTime SubmissionDate { get; set; }

        /// <summary>
        /// Gets or sets the topic identifier.
        /// </summary>
        /// <value>The topic.</value>
        [XmlAttribute("topic")]
        public int TopicID { get; set; }

        /// <summary>
        /// Gets the slots.
        /// </summary>
        /// <value>The slots.</value>
        [XmlElement("slot")]
        public Collection<SerializableLoadoutSlot> Slots
        {
            get { return m_slots; }
        }
    }
}