using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Settings for EVE Mail Messages.
    /// </summary>
    /// <remarks>
    /// This is the optimized way to implement the object as serializable and satisfy all FxCop rules.
    /// Don't use auto-property with private setter for the collections as it does not work with XmlSerializer.
    /// </remarks>
    public sealed class EveMailMessagesSettings
    {
        private Collection<EveMailMessagesColumnSettings> m_columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailMessagesSettings"/> class.
        /// </summary>
        public EveMailMessagesSettings()
        {
            m_columns = new Collection<EveMailMessagesColumnSettings>();

            ReadingPanePosition = ReadingPanePositioning.Off;
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public Collection<EveMailMessagesColumnSettings> Columns
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
        public IEnumerable<EveMailMessagesColumnSettings> DefaultColumns
        {
            get
            {
                EveMailMessagesColumn[] defaultColumns = new[]
                                                             {
                                                                 EveMailMessagesColumn.SenderName,
                                                                 EveMailMessagesColumn.Title,
                                                                 EveMailMessagesColumn.SentDate,
                                                                 EveMailMessagesColumn.ToCharacters
                                                             };

                List<EveMailMessagesColumnSettings> researchColumns = Columns.ToList();
                researchColumns.AddRange(EnumExtensions.GetValues<EveMailMessagesColumn>().Where(
                    x => x != EveMailMessagesColumn.None).Where(x => researchColumns.All(y => y.Column != x)).Select(
                        x => new EveMailMessagesColumnSettings
                                 {
                                     Column = x,
                                     Visible = defaultColumns.Contains(x),
                                     Width = -1
                                 }));

                return researchColumns;
            }
        }

        /// <summary>
        /// Adds the specified columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public void Add(List<EveMailMessagesColumnSettings> columns)
        {
            m_columns = new Collection<EveMailMessagesColumnSettings>(columns);
        }
    }
}