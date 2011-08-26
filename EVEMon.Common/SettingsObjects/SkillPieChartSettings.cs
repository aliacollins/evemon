using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class SkillPieChartSettings
    {
        public SkillPieChartSettings()
        {
            Colors = new List<SerializableColor>();
        }

        [XmlElement("sortBySize")]
        public bool SortBySize { get; set; }

        [XmlElement("mergeMinorGroups")]
        public bool MergeMinorGroups { get; set; }

        [XmlElement("sliceHeight")]
        public float SliceHeight { get; set; }

        [XmlElement("initialAngle")]
        public float InitialAngle { get; set; }

        [XmlElement("colors")]
        public List<SerializableColor> Colors { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal SkillPieChartSettings Clone()
        {
            SkillPieChartSettings clone = new SkillPieChartSettings
                                              {
                                                  MergeMinorGroups = MergeMinorGroups,
                                                  InitialAngle = InitialAngle,
                                                  SliceHeight = SliceHeight,
                                                  SortBySize = SortBySize
                                              };
            clone.Colors.AddRange(Colors.Select(x => x.Clone()));
            return clone;
        }
    }
}
