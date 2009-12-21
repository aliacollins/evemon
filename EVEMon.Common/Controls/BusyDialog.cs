using System;
using System.Threading;
using System.Windows.Forms;

namespace EVEMon.Common.Controls
{
    public partial class BusyDialog : EVEMonForm
    {
        private static object m_lockObj = new object();

        public BusyDialog()
        {
            InitializeComponent();
        }
    }
}