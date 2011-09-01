using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class ExportationSettings
    {
        public ExportationSettings()
        {
            PlanToText = new PlanExportSettings();
        }

        [XmlElement("planToText")]
        public PlanExportSettings PlanToText { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal ExportationSettings Clone()
        {
            ExportationSettings clone = (ExportationSettings)MemberwiseClone();
            clone.PlanToText = PlanToText.Clone();
            return clone;
        }
    }
}