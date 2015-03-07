using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class ShipBrowserSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShipBrowserSettings"/> class.
        /// </summary>
        public ShipBrowserSettings()
        {
            UsabilityFilter = ObjectUsabilityFilter.All;
            RacesFilter = Race.All;
        }

        /// <summary>
        /// Gets or sets the usability filter.
        /// </summary>
        /// <value>The usability filter.</value>
        [XmlElement("usabilityFilter")]
        public ObjectUsabilityFilter UsabilityFilter { get; set; }

        /// <summary>
        /// Gets or sets the races filter.
        /// </summary>
        /// <value>The races filter.</value>
        [XmlElement("racesFilter")]
        public Race RacesFilter { get; set; }

        /// <summary>
        /// Gets or sets the text search.
        /// </summary>
        /// <value>The text search.</value>
        [XmlElement("textSearch")]
        public string TextSearch { get; set; }
    }
}