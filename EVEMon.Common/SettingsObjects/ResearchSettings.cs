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
        private readonly Collection<ResearchColumnSettings> m_columns;

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

                return EnumExtensions.GetValues<ResearchColumn>().Where(
                    planColumn => planColumn != ResearchColumn.None).Where(
                        planColumn => Columns.All(columnSetting => columnSetting.Column != planColumn)).Select(
                            planColumn => new ResearchColumnSettings
                                              {
                                                  Column = planColumn,
                                                  Visible = defaultColumns.Contains(planColumn),
                                                  Width = -2
                                              });
            }
        }
    }
}