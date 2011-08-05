using System.Windows.Forms;

namespace EVEMon.Controls
{
    public class NoFlickerPanel : Panel
    {
        public NoFlickerPanel()
        {
            DoubleBuffered = true;
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

