using System.Windows.Forms;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class KillLogColumnSettings : IColumnSettings
    {
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>The column.</value>
        [XmlAttribute("column")]
        public ColumnHeader Column { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WalletTransactionColumnSettings"/> is visible.
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
            get { return Column.DisplayIndex; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Column.Text;
        }
    }
}
