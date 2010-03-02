using System;

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
