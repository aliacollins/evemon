using System.Collections.ObjectModel;
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
        private readonly Collection<OldSettingsCharacter> m_characters;
        private readonly Collection<OldSettingsPlan> m_plans;

        public OldSettings()
        {
            m_characters = new Collection<OldSettingsCharacter>();
            m_plans = new Collection<OldSettingsPlan>();
        }

        [XmlArray("characters")]
        [XmlArrayItem("character")]
        public Collection<OldSettingsCharacter> Characters
        {
            get { return m_characters; }
        }

        [XmlArray("plans")]
        [XmlArrayItem("plan")]
        public Collection<OldSettingsPlan> Plans
        {
            get { return m_plans; }
        }
    }
}