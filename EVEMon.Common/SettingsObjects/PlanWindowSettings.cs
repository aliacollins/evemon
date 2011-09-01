using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class PlanWindowSettings
    {
        public PlanWindowSettings()
        {
            DimUntrainable = true;
            OnlyShowSelectionSummaryOnMultiSelect = true;

            PrioritiesMsgBox = new CustomMsgBoxSettings();
            ObsoleteEntryRemovalBehaviour = ObsoleteEntryRemovalBehaviour.RemoveConfirmed;

            PlanColumn[] displayedColumns = new[]
                                                {
                                                    PlanColumn.Priority,
                                                    PlanColumn.SkillName,
                                                    PlanColumn.TrainingTime,
                                                    PlanColumn.SkillRank,
                                                    PlanColumn.PrimaryAttribute,
                                                    PlanColumn.SecondaryAttribute,
                                                    PlanColumn.SkillGroup,
                                                    PlanColumn.SPPerHour
                                                };

            Columns = EnumExtensions.GetValues<PlanColumn>().Where(
                x => x != PlanColumn.None).Select(x =>
                                                  new PlanColumnSettings
                                                      { Column = x, Visible = displayedColumns.Contains(x), Width = -1 }).ToArray();
        }

        [XmlElement("highlightPlannedSkills")]
        public bool HighlightPlannedSkills { get; set; }

        [XmlElement("highlightPrerequisites")]
        public bool HighlightPrerequisites { get; set; }

        [XmlElement("highlightConflicts")]
        public bool HighlightConflicts { get; set; }

        [XmlElement("highlightPartialSkills")]
        public bool HighlightPartialSkills { get; set; }

        [XmlElement("highlightQueuedSkills")]
        public bool HighlightQueuedSkills { get; set; }

        [XmlElement("onlyShowSelectionSummaryOnMultiSelect")]
        public bool OnlyShowSelectionSummaryOnMultiSelect { get; set; }

        [XmlElement("useAdvanceEntryAddition")]
        public bool UseAdvanceEntryAddition { get; set; }

        [XmlElement("dimUntrainable")]
        public bool DimUntrainable { get; set; }

        [XmlElement("prioritiesMsgBox")]
        public CustomMsgBoxSettings PrioritiesMsgBox { get; set; }

        [XmlElement("obsoleteEntryRemovalBehaviour")]
        public ObsoleteEntryRemovalBehaviour ObsoleteEntryRemovalBehaviour { get; set; }

        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public PlanColumnSettings[] Columns { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public PlanWindowSettings Clone()
        {
            PlanWindowSettings clone = (PlanWindowSettings)MemberwiseClone();
            clone.Columns = Columns.Select(x => x.Clone()).ToArray();
            clone.PrioritiesMsgBox = PrioritiesMsgBox.Clone();
            return clone;
        }
    }
}