using System;
using System.Windows.Forms;

using EVEMon.Common.Data;

namespace EVEMon.SkillPlanner
{
    public partial class ShipBrowserControl : EveObjectBrowserSimple
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ShipBrowserControl()
        {
            InitializeComponent();
            this.scObjectBrowser.RememberDistanceKey = "ShipsBrowser_Left";
            this.Initialize(lvShipProperties, shipSelectControl, true);
        }

        protected override void OnSelectionChanged(object sender, EventArgs e)
        {
            base.OnSelectionChanged(sender, e);
            if (SelectedObject == null) return;

            var loadoutSelect = WindowsFactory<ShipLoadoutSelectWindow>.GetUnique();
            if (loadoutSelect != null) loadoutSelect.Ship = (Item)shipSelectControl.SelectedObject;
        }

        private void lblBattleclinic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var window = WindowsFactory<ShipLoadoutSelectWindow>.ShowUnique(() => new ShipLoadoutSelectWindow(SelectedObject as Item, Plan));
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            ListViewExporter.CreateCSV(lvShipProperties);
        }


    }
}


