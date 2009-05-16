using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Globalization;

namespace EVEMon.Common
{
    /// <summary>
    /// A simple font helper factory.
    /// </summary>
    public static class FontHelper
    {
        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <value>The default font.</value>
        private static Font DefaultFont
        {
            get
            {
                return SystemFonts.DefaultFont;
            }
        }

        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <returns></returns>
        public static Font GetDefaultFont()
        {
            return FontHelper.DefaultFont;
        }

        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <param name="emSize">Size of the em.</param>
        /// <returns></returns>
        public static Font GetDefaultFont(float emSize)
        {
            return FontHelper.GetDefaultFont(emSize, FontStyle.Regular);
        }

        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public static Font GetDefaultFont(FontStyle style)
        {
            return FontHelper.GetDefaultFont(SystemFonts.DefaultFont.Size, style);
        }

        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <param name="emSize">Size of the em.</param>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public static Font GetDefaultFont(float emSize, FontStyle style)
        {
            Font newFont = null;

            try
            {
                newFont = new Font(FontHelper.DefaultFont.FontFamily, emSize, style);
            }
            catch (ArgumentException e)
            {
                // An ArgumentException is thrown when a Font doens't
                // support a requested feature (e.g. the FontStyle).
                // Maybe there's a better way to check if the font
                // would support the style. I'm looking for something
                // like Font.SupportsStyle(..). So we're just using the
                // basic font without any styles. 
                newFont = new Font(FontHelper.DefaultFont.FontFamily, emSize);
                ExceptionHandler.LogException(e, true);
            }

            // Now we're enforcing that newFont must be properly constructed.
            // Maybe the check isn't needed ..
            string errorMessage = String.Format(CultureInfo.CurrentCulture, "The default Operating System Font {0} can't be used.", DefaultFont.Name);
            Enforce.NotNull(newFont, errorMessage);

            return newFont;
        }
    }
}
