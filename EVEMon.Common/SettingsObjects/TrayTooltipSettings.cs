using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class TrayTooltipSettings
    {
        public TrayTooltipSettings()
        {
            Format = "%n - %s %tr - %r";
        }

        [XmlElement("format")]
        public string Format { get; set; }

        [XmlElement("DisplayOrder")]
        public bool DisplayOrder { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public TrayTooltipSettings Clone()
        {
            return (TrayTooltipSettings)MemberwiseClone();
        }
    }
}
