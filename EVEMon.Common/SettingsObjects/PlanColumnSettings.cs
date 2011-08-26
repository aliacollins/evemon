using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    public sealed class PlanColumnSettings : IColumnSettings
    {
        public PlanColumnSettings()
        {
            Width = -1;
        }

        [XmlAttribute("column")]
        public PlanColumn Column { get; set; }

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
        public PlanColumnSettings Clone()
        {
            return (PlanColumnSettings)MemberwiseClone();
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
