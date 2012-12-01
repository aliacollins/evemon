using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.Controls
{
    public partial class KillReportFittingContent : UserControl
    {
        private KillLog m_killLog;

        public KillReportFittingContent()
        {
            InitializeComponent();

            SaveFittingButton.Visible = false;
        }

        internal KillLog KillLog
        {
            get { return m_killLog; }
            set
            {
                m_killLog = value;
                UpdateContent();
            }
        }

        private void UpdateContent()
        {
        }
    }
}
