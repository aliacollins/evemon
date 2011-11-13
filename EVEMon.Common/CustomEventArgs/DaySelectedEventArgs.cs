using System;
using System.Drawing;
using System.Windows.Forms;

namespace EVEMon.Common.CustomEventArgs
{
    public sealed class DaySelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <param name="mouse">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="loc">The loc.</param>
        public DaySelectedEventArgs(DateTime datetime, MouseEventArgs mouse, Point loc)
        {
            DateTime = datetime;
            Mouse = mouse;
            Location = loc;
        }

        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>The date time.</value>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Gets or sets the mouse.
        /// </summary>
        /// <value>The mouse.</value>
        public MouseEventArgs Mouse { get; private set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location { get; private set; }
    }
}
