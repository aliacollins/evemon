using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class ExportationSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportationSettings"/> class.
        /// </summary>
        public ExportationSettings()
        {
            PlanToText = new PlanExportSettings();
        }

        /// <summary>
        /// Gets or sets the plan to text.
        /// </summary>
        /// <value>The plan to text.</value>
        [XmlElement("planToText")]
        public PlanExportSettings PlanToText { get; set; }
    }
}