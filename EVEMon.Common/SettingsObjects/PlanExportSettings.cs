using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Represents the set of options for plan exporting
    /// </summary>
    public class PlanExportSettings
    {
        [XmlAttribute("includeHeader")]
        public bool IncludeHeader { get; set; }

        [XmlAttribute("entryNumber")]
        public bool EntryNumber { get; set; }

        [XmlAttribute("entryTrainingTimes")]
        public bool EntryTrainingTimes { get; set; }

        [XmlAttribute("entryStartDate")]
        public bool EntryStartDate { get; set; }

        [XmlAttribute("entryFinishDate")]
        public bool EntryFinishDate { get; set; }

        [XmlAttribute("entryNotes")]
        public bool EntryNotes { get; set; }

        [XmlAttribute("entryCost")]
        public bool EntryCost { get; set; }

        [XmlAttribute("footerCount")]
        public bool FooterCount { get; set; }

        [XmlAttribute("footerTotalTime")]
        public bool FooterTotalTime { get; set; }

        [XmlAttribute("footerDate")]
        public bool FooterDate { get; set; }

        [XmlAttribute("footerCost")]
        public bool FooterCost { get; set; }

        /// <summary>
        /// Output markup type.
        /// </summary>
        [XmlElement("markup")]
        public MarkupType Markup { get; set; }

        /// <summary>
        /// If <code>true</code>, known skills are filtered out.
        /// </summary>
        [XmlElement("shoppingList")]
        public bool ShoppingList { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public PlanExportSettings Clone()
        {
            return (PlanExportSettings)MemberwiseClone();
        }
    }
}
