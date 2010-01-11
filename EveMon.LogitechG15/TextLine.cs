using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace EVEMon.LogitechG15
{
    public class TextLine : ILcdLine
    {
        private string m_text;
        private Font m_font;

        public TextLine(string text, Font font)
        {
            m_text = text;
            m_font = font;
        }

        #region ILcdLine Members

        public float Height
        {
            get;
            private set;
        }

        public void Render(Graphics canvas, Graphics overlay, float offset)
        {
            RectangleF lineRect = new RectangleF(new PointF(0f, offset), canvas.MeasureString(m_text, m_font));
            canvas.DrawString(m_text, m_font, new SolidBrush(Color.Black), lineRect);
            Height = lineRect.Height;
        }

        #endregion
    }
}
