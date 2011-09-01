using System.Linq;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class EveMailMessagesSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailMessagesSettings"/> class.
        /// </summary>
        public EveMailMessagesSettings()
        {
            // Add default columns
            EveMailMessagesColumn[] defaultColumns = new[]
                                                         {
                                                             EveMailMessagesColumn.SenderName,
                                                             EveMailMessagesColumn.Title,
                                                             EveMailMessagesColumn.SentDate,
                                                             EveMailMessagesColumn.ToCharacters
                                                         };

            Columns = EnumExtensions.GetValues<EveMailMessagesColumn>().Where(
                x => x != EveMailMessagesColumn.None).Select(x =>
                                                             new EveMailMessagesColumnSettings
                                                                 {
                                                                     Column = x,
                                                                     Visible = defaultColumns.Contains(x),
                                                                     Width = -1
                                                                 }).ToArray();

            ReadingPanePosition = ReadingPanePositioning.Off;
        }

        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public EveMailMessagesColumnSettings[] Columns { get; set; }

        [XmlElement("readingPanePosition")]
        public ReadingPanePositioning ReadingPanePosition { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public EveMailMessagesSettings Clone()
        {
            EveMailMessagesSettings clone = (EveMailMessagesSettings)MemberwiseClone();
            clone.Columns = Columns.Select(x => x.Clone()).ToArray();
            return clone;
        }
    }
}