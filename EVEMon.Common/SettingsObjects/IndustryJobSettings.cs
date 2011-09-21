using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Settings for Industry Jobs.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class IndustryJobSettings
    {
        private Collection<IndustryJobColumnSettings> m_columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndustryJobSettings"/> class.
        /// </summary>
        public IndustryJobSettings()
        {
            m_columns = new Collection<IndustryJobColumnSettings>();

            HideInactiveJobs = true;
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public Collection<IndustryJobColumnSettings> Columns
        {
            get { return m_columns; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hide inactive jobs].
        /// </summary>
        /// <value><c>true</c> if [hide inactive jobs]; otherwise, <c>false</c>.</value>
        [XmlElement("hideInactiveJobs")]
        public bool HideInactiveJobs { get; set; }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <value>The default columns.</value>
        public IEnumerable<IndustryJobColumnSettings> DefaultColumns
        {
            get
            {
                IndustryJobColumn[] defaultColumns = new[]
                                                         {
                                                             IndustryJobColumn.State,
                                                             IndustryJobColumn.TTC,
                                                             IndustryJobColumn.InstalledItem,
                                                             IndustryJobColumn.OutputItem
                                                         };

                List<IndustryJobColumnSettings> industryJobColumns = Columns.ToList();
                industryJobColumns.AddRange(EnumExtensions.GetValues<IndustryJobColumn>().Where(
                    x => x != IndustryJobColumn.None).Where(x => industryJobColumns.All(y => y.Column != x)).Select(
                            x => new IndustryJobColumnSettings
                                     {
                                         Column = x,
                                         Visible = defaultColumns.Contains(x),
                                         Width = -1
                                     }));

                return industryJobColumns;
            }
        }

        /// <summary>
        /// Adds the specified columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public void Add(List<IndustryJobColumnSettings> columns)
        {
            m_columns = new Collection<IndustryJobColumnSettings>(columns);
        }
    }
}