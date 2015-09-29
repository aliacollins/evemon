using System;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Factories;

namespace EVEMon.PatchXmlCreator
{
    internal partial class DatafileControl : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DatafileControl()
        {
            InitializeComponent();
            dtpDatafiles.Font = FontFactory.GetFont("Tahoma");
            lblDatafileDate.Font = FontFactory.GetFont("Tahoma");
        }

        /// <summary>
        /// Occurs on control click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClick(object sender, EventArgs e)
        {
            Parent.Focus();
        }
    }
}