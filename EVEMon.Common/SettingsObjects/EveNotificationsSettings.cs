using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Settings for EVE Notifications.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class EveNotificationsSettings
    {
        private readonly Collection<EveNotificationsColumnSettings> m_columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveNotificationsSettings"/> class.
        /// </summary>
        public EveNotificationsSettings()
        {
            m_columns = new Collection<EveNotificationsColumnSettings>();

            ReadingPanePosition = ReadingPanePositioning.Off;
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public Collection<EveNotificationsColumnSettings> Columns
        {
            get { return m_columns; }
        }

        /// <summary>
        /// Gets or sets the reading pane position.
        /// </summary>
        /// <value>The reading pane position.</value>
        [XmlElement("readingPanePosition")]
        public ReadingPanePositioning ReadingPanePosition { get; set; }

        /// <summary>
        /// Gets the default columns.
        /// </summary>
        /// <value>The default columns.</value>
        public IEnumerable<EveNotificationsColumnSettings> DefaultColumns
        {
            get
            {
                EveNotificationsColumn[] defaultColumns = new[]
                                                              {
                                                                  EveNotificationsColumn.SenderName,
                                                                  EveNotificationsColumn.Type,
                                                                  EveNotificationsColumn.SentDate
                                                              };

                List<EveNotificationsColumnSettings> eveNotificationsColumns = Columns.ToList();
                eveNotificationsColumns.AddRange(EnumExtensions.GetValues<EveNotificationsColumn>().Where(
                    x => x != EveNotificationsColumn.None).Where(x => eveNotificationsColumns.All(y => y.Column != x)).Select(
                        x => new EveNotificationsColumnSettings
                                 {
                                     Column = x,
                                     Visible = defaultColumns.Contains(x),
                                     Width = -2
                                 }));

                return eveNotificationsColumns;
            }
        }

        /// <summary>
        /// Adds the specified columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public void AddRange(IEnumerable<EveNotificationsColumnSettings> columns)
        {
            m_columns.Clear();
            columns.ToList().ForEach(column => m_columns.Add(column));
        }
    }
}