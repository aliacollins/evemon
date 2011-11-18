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
            if (character == null)
                throw new ArgumentNullException("character");

            CharacterGuid = character.Guid;
            Name = character.Name;
            Settings = character.UISettings;
        }

        /// <summary>
        /// Gets or sets the character GUID.
        /// </summary>
        /// <value>The character GUID.</value>
        [XmlAttribute("characterGuid")]
        public Guid CharacterGuid { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        [XmlElement("settings")]
        public CharacterUISettings Settings { get; set; }
    }
}