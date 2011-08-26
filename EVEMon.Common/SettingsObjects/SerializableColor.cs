using System.Drawing;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Represents a color in the settings
    /// </summary>
    public sealed class SerializableColor
    {
        public SerializableColor()
        {
            A = 255;
        }

        [XmlAttribute]
        public byte A { get; set; }

        [XmlAttribute]
        public byte R { get; set; }

        [XmlAttribute]
        public byte G { get; set; }

        [XmlAttribute]
        public byte B { get; set; }


        #region Implciit conversion operators with System.Drawing.Color

        /// <summary>
        /// Performs an explicit conversion from <see cref="EVEMon.Common.SettingsObjects.SerializableColor"/> to <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks>Do not make the conversion operators implicit, there is a bug with XML serialization</remarks>
        public static explicit operator Color(SerializableColor src)
        {
            return Color.FromArgb(src.A, src.R, src.G, src.B);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Drawing.Color"/> to <see cref="EVEMon.Common.SettingsObjects.SerializableColor"/>.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks>Do not make the conversion operators implicit, there is a bug with XML serialization</remarks>
        public static explicit operator SerializableColor(Color src)
        {
            return new SerializableColor {A = src.A, R = src.R, G = src.G, B = src.B};
        }

        #endregion


        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal SerializableColor Clone()
        {
            return (SerializableColor)MemberwiseClone();
        }
    }
}
