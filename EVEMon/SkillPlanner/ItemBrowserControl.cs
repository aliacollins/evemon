using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using System.IO;
using System.Text.RegularExpressions;

namespace EVEMon.SkillPlanner
{
    public partial class ItemBrowserControl : EveObjectBrowserSimple
    {
        public ItemBrowserControl()
        {
            InitializeComponent();
            this.scObjectBrowser.RememberDistanceKey = "ItemBrowser_Left";
            this.Initialize(lvItemProperties, itemSelectControl, false);
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            ListViewExporter.CreateCSV(lvItemProperties);
        }

    }
}
