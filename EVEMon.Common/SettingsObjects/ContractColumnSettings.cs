using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class ContractColumnSettings : IColumnSettings
    {
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column.</value>
        [XmlAttribute("column")]
        public ContractColumn Column { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ContractColumnSettings"/> is visible.
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
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Column.GetHeader();
        }
    }
}
