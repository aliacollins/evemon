using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace EVEMon.LogitechG15
{
    public class ProgressLine : ILcdLine
    {
        private double m_percentage;
        private Font m_font;

        public ProgressLine(double percentage, Font font)
        {
            m_percentage = percentage;
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
            string text = m_percentage.ToString("P2");
            SizeF textSize = canvas.MeasureString(text, m_font);
            
            RectangleF barRect = new RectangleF(new PointF(0f, offset + 1), new SizeF(G15Constants.G15Width - 1, textSize.Height - 2));
            float textLeft = (barRect.Width / 2) - (textSize .Width / 2);
            RectangleF textRect = new RectangleF(new PointF(textLeft, offset), canvas.MeasureString(text, m_font));

            int barFill = Convert.ToInt16(m_percentage * G15Constants.G15Width - 2);

            canvas.DrawRectangle(new Pen(Color.Black), barRect.Left, barRect.Top, barRect.Width, barRect.Height);
            canvas.FillRectangle(new SolidBrush(Color.Black), barRect.Left + 1, barRect.Top + 1, barFill, barRect.Height - 2);
            overlay.DrawString(text, m_font, new SolidBrush(Color.Black), textRect);
            Height = barRect.Height + 1;
        }

        #endregion
    }
}
