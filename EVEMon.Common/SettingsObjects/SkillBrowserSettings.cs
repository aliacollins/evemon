using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class SkillBrowserSettings
    {
        [XmlElement("filter")]
        public SkillFilter Filter { get; set; }

        [XmlElement("sort")]
        public SkillSort Sort { get; set; }

        [XmlElement("textSearch")]
        public string TextSearch { get; set; }

        [XmlElement("iconsGroupIndex")]
        public int IconsGroupIndex { get; set; }

        [XmlElement("showNonPublicSkills")]
        public bool ShowNonPublicSkills { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal SkillBrowserSettings Clone()
        {
            return (SkillBrowserSettings)MemberwiseClone();
        }
    }
}
