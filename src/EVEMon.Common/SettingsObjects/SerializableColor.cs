using System;
using System.Drawing;
using System.Xml.Serialization;

namespace EVEMon.Common.SettingsObjects
{
    /// <summary>
    /// Represents a color in the settings
    /// </summary>
    public sealed class SerializableColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableColor"/> class.
        /// </summary>
        public SerializableColor()
        {
            A = 255;
        }

        /// <summary>
        /// Gets or sets a.
        /// </summary>
        /// <value>
        /// a.
        /// </value>
        [XmlAttribute]
        public byte A { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        /// The r.
        /// </value>
        [XmlAttribute]
        public byte R { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        /// The g.
        /// </value>
        [XmlAttribute]
        public byte G { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        /// The b.
        /// </value>
        [XmlAttribute]
        public byte B { get; set; }


        #region Explicit conversion operators with System.Drawing.Color

        /// <summary>
        /// Performs an explicit conversion from <see cref="EVEMon.Common.SettingsObjects.SerializableColor"/> to <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks>Do not make the conversion operators implicit, there is a bug with XML serialization</remarks>
        public static explicit operator Color(SerializableColor src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            return Color.FromArgb(src.A, src.R, src.G, src.B);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="System.Drawing.Color"/> to <see cref="EVEMon.Common.SettingsObjects.SerializableColor"/>.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks>Do not make the conversion operators implicit, there is a bug with XML serialization</remarks>
        public static explicit operator SerializableColor(Color src)
            => new SerializableColor { A = src.A, R = src.R, G = src.G, B = src.B };

        #endregion
    }
}