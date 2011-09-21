using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic
{
    [XmlRoot("loadouts")]
    public sealed class SerializableLoadoutFeed
    {
        /// <summary>
        /// Gets or sets the race.
        /// </summary>
        /// <value>The race.</value>
        [XmlElement("race")]
        public SerializableLoadoutRace Race { get; set; }
    }
}