namespace EVEMon.CharacterMonitoring
{
    partial class CharacterPlanetaryList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterPlanetaryList));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.noPlanetaryColoniesLabel = new System.Windows.Forms.Label();
            this.lvPlanetary = new System.Windows.Forms.ListView();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToCSVToolStripMenuItem});
            this.contextMenu.Name = "ShipPropertiesContextMenu";
            this.contextMenu.Size = new System.Drawing.Size(158, 26);
            // 
            // exportToCSVToolStripMenuItem
            // 
            this.exportToCSVToolStripMenuItem.Name = "exportToCSVToolStripMenuItem";
            this.exportToCSVToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.exportToCSVToolStripMenuItem.Text = "Export To CSV...";
            this.exportToCSVToolStripMenuItem.Click += new System.EventHandler(this.exportToCSVToolStripMenuItem_Click);
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "arrow_up.png");
            this.ilIcons.Images.SetKeyName(1, "arrow_down.png");
            this.ilIcons.Images.SetKeyName(2, "16x16Transparant.png");
            // 
            // noPlanetaryColoniesLabel
            // 
            this.noPlanetaryColoniesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noPlanetaryColoniesLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.noPlanetaryColoniesLabel.Location = new System.Drawing.Point(0, 0);
            this.noPlanetaryColoniesLabel.Name = "noPlanetaryColoniesLabel";
            this.noPlanetaryColoniesLabel.Size = new System.Drawing.Size(454, 434);
            this.noPlanetaryColoniesLabel.TabIndex = 2;
            this.noPlanetaryColoniesLabel.Text = "No planetary info are available.";
            this.noPlanetaryColoniesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvPlanetaryColonies
            // 
            this.lvPlanetary.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvPlanetary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPlanetary.FullRowSelect = true;
            this.lvPlanetary.HideSelection = false;
            this.lvPlanetary.Location = new System.Drawing.Point(0, 0);
            this.lvPlanetary.MultiSelect = false;
            this.lvPlanetary.Name = "lvPlanetary";
            this.lvPlanetary.Size = new System.Drawing.Size(454, 434);
            this.lvPlanetary.SmallImageList = this.ilIcons;
            this.lvPlanetary.TabIndex = 3;
            this.lvPlanetary.UseCompatibleStateImageBehavior = false;
            this.lvPlanetary.View = System.Windows.Forms.View.Details;
            // 
            // CharacterPlanetaryList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvPlanetary);
            this.Controls.Add(this.noPlanetaryColoniesLabel);
            this.Name = "CharacterPlanetaryList";
            this.Size = new System.Drawing.Size(454, 434);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem exportToCSVToolStripMenuItem;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.Label noPlanetaryColoniesLabel;
        private System.Windows.Forms.ListView lvPlanetary;
    }
}
