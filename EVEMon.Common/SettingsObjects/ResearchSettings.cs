using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Settings for Research.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class ResearchSettings
    {
        private Collection<ResearchColumnSettings> m_columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResearchSettings"/> class.
        /// </summary>
        public ResearchSettings()
        {
            m_columns = new Collection<ResearchColumnSettings>();
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public Collection<ResearchColumnSettings> Columns
        {
            get { return m_columns; }
        }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <value>The default columns.</value>
        public IEnumerable<ResearchColumnSettings> DefaultColumns
        {
            get
            {
                ResearchColumn[] defaultColumns = new[]
                                                      {
                                                          ResearchColumn.Agent,
                                                          ResearchColumn.Field,
                                                          ResearchColumn.CurrentRP,
                                                          ResearchColumn.PointsPerDay,
                                                          ResearchColumn.Station
                                                      };

                List<ResearchColumnSettings> researchColumns = Columns.ToList();
                researchColumns.AddRange(EnumExtensions.GetValues<ResearchColumn>().Where(
                    x => x != ResearchColumn.None).Where(x => researchColumns.All(y => y.Column != x)).Select(
                            x => new ResearchColumnSettings
                                     {
                                         Column = x,
                                         Visible = defaultColumns.Contains(x),
                                         Width = -2
                                     }));

                return researchColumns;
            }
        }

        /// <summary>
        /// Adds the specified columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public void Add(List<ResearchColumnSettings> columns)
        {
            m_columns = new Collection<ResearchColumnSettings>(columns);
        }
    }
}