using System;

namespace EVEMon.Common
{
    public class EveServerEventArgs : EventArgs
    {
        public EveServerEventArgs(string info, System.Windows.Forms.ToolTipIcon icon)
        {
            this.info = info;
            this.icon = icon;
        }

        public EveServerEventArgs(string info)
        {
            this.info = info;
        }

        public string info;
        public System.Windows.Forms.ToolTipIcon icon;
    }
}