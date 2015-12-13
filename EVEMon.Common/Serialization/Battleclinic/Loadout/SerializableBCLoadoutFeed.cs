using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.BattleClinic.Loadout
{
    [XmlRoot("loadouts")]
    public sealed class SerializableBCLoadoutFeed
    {
        /// <summary>
        /// Gets or sets the race.
        /// </summary>
        /// <value>The race.</value>
        [XmlElement("race")]
        public SerializableBCLoadoutRace Race { get; set; }
    }
}