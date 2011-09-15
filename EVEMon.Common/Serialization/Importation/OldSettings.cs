using System.Collections.Generic;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Importation
{
    /// <summary>
    /// Facilitates importation of characters from the settings XML 
    /// of versions of EVEMon prior to 1.3.0.
    /// </summary>
    /// <remarks>
    /// These changes were released early 2010, it is safe to assume
    /// that they can be removed from the project early 2012.
    /// </remarks>
    [XmlRoot("OldSettings")]
    public sealed class OldSettings
    {
        public OldSettings()
        {
            Characters = new List<OldSettingsCharacter>();
            Plans = new List<OldSettingsPlan>();
        }

        [XmlArray("characters")]
        [XmlArrayItem("character")]
        public List<OldSettingsCharacter> Characters { get; set; }

        [XmlArray("plans")]
        [XmlArrayItem("plan")]
        public List<OldSettingsPlan> Plans { get; set; }
    }
}