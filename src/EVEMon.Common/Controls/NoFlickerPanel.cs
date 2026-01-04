using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    public class NoFlickerPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoFlickerPanel"/> class.
        /// </summary>
        public NoFlickerPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.ContainerControl |
                     ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }
    }
}