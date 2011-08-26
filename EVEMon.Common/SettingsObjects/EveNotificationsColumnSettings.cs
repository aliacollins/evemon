using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class EveNotificationsColumnSettings : IColumnSettings
    {
        public EveNotificationsColumnSettings()
        {
            Width = -1;
        }

        [XmlAttribute("column")]
        public EveNotificationsColumn Column { get; set; }

        [XmlAttribute("visible")]
        public bool Visible { get; set; }

        [XmlAttribute("width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        int IColumnSettings.Key
        {
            get { return (int)Column; }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public EveNotificationsColumnSettings Clone()
        {
            return (EveNotificationsColumnSettings)MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the column's header text.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the column's header text.
        /// </returns>
        public override string ToString()
        {
            return Column.GetHeader();
        }
    }
}