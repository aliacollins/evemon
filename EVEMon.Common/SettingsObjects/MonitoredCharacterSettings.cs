using System;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class MonitoredCharacterSettings
    {
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        public MonitoredCharacterSettings()
        {
            Settings = new CharacterUISettings();
        }

        /// <summary>
        /// Creation for new settings for this character.
        /// </summary>
        public MonitoredCharacterSettings(Character character)
        {
            CharacterGuid = character.Guid;
            Name = character.Name;
            Settings = character.UISettings.Clone();
        }

        [XmlAttribute("characterGuid")]
        public Guid CharacterGuid { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("settings")]
        public CharacterUISettings Settings { get; set; }
    }
}