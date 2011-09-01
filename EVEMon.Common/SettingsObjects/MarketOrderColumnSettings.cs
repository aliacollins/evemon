using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class MarketOrderColumnSettings : IColumnSettings
    {
        public MarketOrderColumnSettings()
        {
            Width = -1;
        }

        [XmlAttribute("column")]
        public MarketOrderColumn Column { get; set; }

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
        public MarketOrderColumnSettings Clone()
        {
            return new MarketOrderColumnSettings { Column = Column, Visible = Visible, Width = Width };
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