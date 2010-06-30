using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using EVEMon.Common;

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
            int left = 0;
            int size = G15Constants.G15Width;
            int pad = 2;

            if (Settings.G15.ShowEVETime)
            {
                string EVETime = DateTime.UtcNow.ToString("HH:mm");
                SizeF EVETimeSize = canvas.MeasureString(EVETime, m_font);
                left = (int)EVETimeSize.Width + pad;
                size = G15Constants.G15Width - left - pad;
            }

            if (Settings.G15.ShowSystemTime)
            {
                string SystemTime = DateTime.Now.ToShortTimeString();
                SizeF SystemTimeSize = canvas.MeasureString(SystemTime, m_font);
                
                size = size - (int)SystemTimeSize.Width;
            }


            RectangleF barRect = new RectangleF(new PointF(left, offset + 1), new SizeF(size - 1, textSize.Height - 2));
            float textLeft = (barRect.Width / 2) - (textSize.Width / 2);
            RectangleF textRect = new RectangleF(new PointF(left + textLeft, offset), canvas.MeasureString(text, m_font));

            int barFill = Convert.ToInt16(m_percentage * size - 2);

            canvas.DrawRectangle(new Pen(Color.Black), barRect.Left, barRect.Top, barRect.Width, barRect.Height);
            canvas.FillRectangle(new SolidBrush(Color.Black), barRect.Left + 1, barRect.Top + 1, barFill, barRect.Height - 2);
            overlay.DrawString(text, m_font, new SolidBrush(Color.Black), textRect);
            Height = barRect.Height + 1;
        }

        #endregion
    }
}
