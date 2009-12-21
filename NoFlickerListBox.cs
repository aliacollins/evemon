using System.Drawing;
using System.Windows.Forms;

namespace EVEMon
{
    public class NoFlickerListBox : ListBox
    {
        /*
        // This code that has been rem'd out is what I'm currently working on....
        // it does some odd stuff atm.
        public NoFlickerListBox()
        {
            //SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.DoubleBuffer, true);
            //SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.ContainerControl, true);
        }

        //protected internal override void OnPreRender() // ??
        //{
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this != null)
            {
                for (int i = this.TopIndex; i < this.TopIndex + 4; i++)
                {

                }
                base.OnPaint(e);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
         */

        private enum WM
        {
            WM_NULL = 0x0000,
            WM_ERASEBKGND = 0x0014
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)WM.WM_ERASEBKGND:
                    PaintNonItemRegion();
                    m.Msg = (int)WM.WM_NULL;
                    break;
            }
            base.WndProc(ref m);
        }

        private void PaintNonItemRegion()
        {
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            using (Region r = new Region(this.ClientRectangle))
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    Rectangle itemRect = this.GetItemRectangle(i);
                    r.Exclude(itemRect);
                }
                g.FillRegion(SystemBrushes.Window, r);
            }
        }
    }
}