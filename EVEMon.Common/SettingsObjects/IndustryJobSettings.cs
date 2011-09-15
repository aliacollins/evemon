using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Settings for Industry Jobs
    /// </summary>
    public sealed class IndustryJobSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndustryJobSettings"/> class.
        /// </summary>
        public IndustryJobSettings()
        {
            // Add default columns
            IndustryJobColumn[] defaultColumns = new[]
                                                     {
                                                         IndustryJobColumn.State,
                                                         IndustryJobColumn.TTC,
                                                         IndustryJobColumn.InstalledItem,
                                                         IndustryJobColumn.OutputItem
                                                     };

            Columns = EnumExtensions.GetValues<IndustryJobColumn>().Where(
                x => x != IndustryJobColumn.None).Select(x =>
                                                         new IndustryJobColumnSettings
                                                             {
                                                                 Column = x,
                                                                 Visible = defaultColumns.Contains(x),
                                                                 Width = -1
                                                             }).ToArray();

            HideInactiveJobs = true;
        }

        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public IndustryJobColumnSettings[] Columns { get; set; }

        [XmlElement("hideInactiveJobs")]
        public bool HideInactiveJobs { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public IndustryJobSettings Clone()
        {
            IndustryJobSettings clone = (IndustryJobSettings)MemberwiseClone();
            clone.Columns = Columns.Select(x => x.Clone()).ToArray();
            return clone;
        }
    }
}