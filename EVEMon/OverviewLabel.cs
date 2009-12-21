using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace EVEMon
{
    /// <summary>
    /// A panel that does not display a gray text when disabled (and we need to disable them so that the button does not always lose focus and its nice "I'm hovered" color)
    /// </summary>
    public sealed class OverviewLabel : Label
    {
        public OverviewLabel()
        {
            this.DoubleBuffered = true;
        }

        public new Boolean Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var foreground = new SolidBrush(this.ForeColor))
            {
                var format = new StringFormat();
                if (this.AutoEllipsis) format.Trimming = StringTrimming.EllipsisCharacter;
                e.Graphics.DrawString(this.Text, this.Font, foreground, Padding.Left, Padding.Right, format);
            }
        }
    }
}
