using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class IndustryJobColumnSettings : IColumnSettings
    {
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column.</value>
        [XmlAttribute("column")]
        public IndustryJobColumn Column { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IndustryJobColumnSettings"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [XmlAttribute("visible")]
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
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
        /// Returns a <see cref="System.String"/> that represents the column's header text.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the column's header text.
        /// </returns>
        public override string ToString()
        {
            return Column.GetHeader();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}