using System;
using System.Drawing;
using EVEMon.Common;

namespace EVEMon.LogitechG15
{
    internal sealed class LineProcess
    {
        private readonly double m_percentage;
        private readonly string m_text;
        private readonly Font m_font;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LineProcess"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        private LineProcess(Font font)
        {
            m_font = font;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineProcess"/> class.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <param name="font">The font.</param>
        internal LineProcess(double percentage, Font font)
            : this(font)
        {
            m_percentage = percentage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineProcess"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        internal LineProcess(string text, Font font)
            : this(font)
        {
            m_text = text;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        internal float Height { get; private set; }

        /// <summary>
        /// Renders the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="overlay">The overlay.</param>
        /// <param name="offset">The offset.</param>
        internal void Render(Graphics canvas, Graphics overlay, float offset)
        {
            if (m_text == null)
                RenderProgressLine(canvas, overlay, offset);
            else
                RenderTextLine(canvas, offset);
        }

        /// <summary>
        /// Renders the text line.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="offset">The offset.</param>
        private void RenderTextLine(Graphics canvas, float offset)
        {
            RectangleF lineRect = new RectangleF(new PointF(0f, offset), canvas.MeasureString(m_text, m_font));
            using (Brush brush = new SolidBrush(Color.Black))
            {
                canvas.DrawString(m_text, m_font, brush, lineRect);
            }

            Height = lineRect.Height;
        }

        /// <summary>
        /// Renders the progress line.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="overlay">The overlay.</param>
        /// <param name="offset">The offset.</param>
        private void RenderProgressLine(Graphics canvas, Graphics overlay, float offset)
        {
            string text = m_percentage.ToString("P2", CultureConstants.DefaultCulture);
            SizeF textSize = canvas.MeasureString(text, m_font);
            int left = 0;
            int width = LcdDisplay.G15Width - 1;
            const int Pad = 2;

            if (Settings.G15.ShowEVETime)
            {
                string eveTime = EveMonClient.EVEServer.ServerDateTime.ToShortTimeString();
                SizeF eveTimeSize = canvas.MeasureString(eveTime, m_font);
                RectangleF eveTimeRect = new RectangleF(new PointF(left, offset), eveTimeSize);
                left = (int)eveTimeSize.Width + Pad;
                width -= left;
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    canvas.DrawString(eveTime, m_font, brush, eveTimeRect);
                }
            }

            if (Settings.G15.ShowSystemTime)
            {
                string systemTime = DateTime.Now.ToShortTimeString();
                SizeF systemTimeSize = canvas.MeasureString(systemTime, m_font);
                RectangleF systemTimeRect = new RectangleF(new PointF(LcdDisplay.G15Width + 1 - systemTimeSize.Width, offset),
                                                                      systemTimeSize);
                width -= (int)systemTimeSize.Width + Pad;
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    canvas.DrawString(systemTime, m_font, brush, systemTimeRect);
                }
            }
            
            RectangleF barRect = new RectangleF(new PointF(left, offset - 1), new SizeF(width, textSize.Height - 1));
            float textLeft = (barRect.Width - textSize.Width) / 2;
            RectangleF textRect = new RectangleF(new PointF(left + textLeft, offset), textSize);

            int barFill = Convert.ToInt16(m_percentage * width - Pad);

            using (Pen pen = new Pen(Color.Black))
            {
                canvas.DrawRectangle(pen, barRect.Left, barRect.Top, barRect.Width, barRect.Height);
            }

            using (Brush brush = new SolidBrush(Color.Black))
            {
                var progressFont = FontFactory.GetFont(m_font.FontFamily, m_font.Size);
                canvas.FillRectangle(brush, barRect.Left + 1, barRect.Top + 1, barFill, barRect.Height - Pad);
                overlay.DrawString(text, progressFont, brush, textRect);
            }

            Height = barRect.Height + 1;
        }

        #endregion
    }
}