using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class IndustryJobColumnSettings : IColumnSettings
    {
        public IndustryJobColumnSettings()
        {
            Width = -1;
        }

        [XmlAttribute("column")]
        public IndustryJobColumn Column { get; set; }

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
        public IndustryJobColumnSettings Clone()
        {
            return (IndustryJobColumnSettings)MemberwiseClone();
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