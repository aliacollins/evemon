namespace EVEMon.SkillPlanner
{
    partial class ShipBrowserControl
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
            this.scShipSelect = new EVEMon.SkillPlanner.PersistentSplitContainer();
            this.lblBattleclinic = new System.Windows.Forms.LinkLabel();
            this.gbAttributes = new System.Windows.Forms.GroupBox();
            this.lvShipProperties = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.shipSelectControl = new EVEMon.SkillPlanner.ShipSelectControl();
            this.ShipPropertiesContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbDescription.SuspendLayout();
            this.gbRequiredSkills.SuspendLayout();
            this.scDetailsRight.Panel1.SuspendLayout();
            this.scDetailsRight.Panel2.SuspendLayout();
            this.scDetailsRight.SuspendLayout();
            this.scDetails.Panel1.SuspendLayout();
            this.scDetails.Panel2.SuspendLayout();
            this.scDetails.SuspendLayout();
            this.scObjectBrowser.Panel1.SuspendLayout();
            this.scObjectBrowser.Panel2.SuspendLayout();
            this.scObjectBrowser.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.pnlBrowserHeader.SuspendLayout();
            this.scShipSelect.SuspendLayout();
            this.gbAttributes.SuspendLayout();
            this.ShipPropertiesContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // scDetailsRight
            // 
            // 
            // scDetails
            // 
            // 
            // scDetails.Panel1
            // 
            this.scDetails.Panel1.Controls.Add(this.gbAttributes);
            // 
            // scObjectBrowser
            // 
            // 
            // scObjectBrowser.Panel1
            // 
            this.scObjectBrowser.Panel1.Controls.Add(this.shipSelectControl);
            // 
            // scObjectBrowser.Panel2
            // 
            this.scObjectBrowser.Panel2.Controls.Add(this.scShipSelect);
            // 
            // pnlBrowserHeader
            // 
            this.pnlBrowserHeader.Controls.Add(this.lblBattleclinic);
            this.pnlBrowserHeader.Controls.SetChildIndex(this.lblEveObjName, 0);
            this.pnlBrowserHeader.Controls.SetChildIndex(this.lblBattleclinic, 0);
            this.pnlBrowserHeader.Controls.SetChildIndex(this.lblEveObjCategory, 0);
            this.pnlBrowserHeader.Controls.SetChildIndex(this.eoImage, 0);
            // 
            // scShipSelect
            // 
            this.scShipSelect.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scShipSelect.Location = new System.Drawing.Point(57, 103);
            this.scShipSelect.Margin = new System.Windows.Forms.Padding(2);
            this.scShipSelect.Name = "scShipSelect";
            this.scShipSelect.RememberDistanceKey = null;
            this.scShipSelect.Size = new System.Drawing.Size(650, 413);
            this.scShipSelect.SplitterDistance = 163;
            this.scShipSelect.SplitterWidth = 5;
            this.scShipSelect.TabIndex = 1;
            // 
            // lblBattleclinic
            // 
            this.lblBattleclinic.AutoSize = true;
            this.lblBattleclinic.Location = new System.Drawing.Point(70, 54);
            this.lblBattleclinic.Name = "lblBattleclinic";
            this.lblBattleclinic.Size = new System.Drawing.Size(101, 13);
            this.lblBattleclinic.TabIndex = 11;
            this.lblBattleclinic.TabStop = true;
            this.lblBattleclinic.Text = "Battleclinic loadouts";
            this.lblBattleclinic.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblBattleclinic_LinkClicked);
            // 
            // gbAttributes
            // 
            this.gbAttributes.Controls.Add(this.lvShipProperties);
            this.gbAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAttributes.Location = new System.Drawing.Point(0, 0);
            this.gbAttributes.Name = "gbAttributes";
            this.gbAttributes.Size = new System.Drawing.Size(238, 343);
            this.gbAttributes.TabIndex = 4;
            this.gbAttributes.TabStop = false;
            this.gbAttributes.Text = "Attributes";
            // 
            // lvShipProperties
            // 
            this.lvShipProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvShipProperties.ContextMenuStrip = this.ShipPropertiesContextMenu;
            this.lvShipProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvShipProperties.FullRowSelect = true;
            this.lvShipProperties.Location = new System.Drawing.Point(3, 16);
            this.lvShipProperties.Name = "lvShipProperties";
            this.lvShipProperties.Size = new System.Drawing.Size(232, 324);
            this.lvShipProperties.TabIndex = 3;
            this.lvShipProperties.UseCompatibleStateImageBehavior = false;
            this.lvShipProperties.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Attribute";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 140;
            // 
            // shipSelectControl
            // 
            this.shipSelectControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.shipSelectControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shipSelectControl.Location = new System.Drawing.Point(0, 0);
            this.shipSelectControl.Margin = new System.Windows.Forms.Padding(2);
            this.shipSelectControl.Name = "shipSelectControl";
            this.shipSelectControl.Plan = null;
            this.shipSelectControl.SelectedObject = null;
            this.shipSelectControl.SelectedObjects = null;
            this.shipSelectControl.Size = new System.Drawing.Size(163, 413);
            this.shipSelectControl.TabIndex = 0;
            // 
            // ShipPropertiesContextMenu
            // 
            this.ShipPropertiesContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToCSVToolStripMenuItem});
            this.ShipPropertiesContextMenu.Name = "ShipPropertiesContextMenu";
            this.ShipPropertiesContextMenu.Size = new System.Drawing.Size(170, 48);
            // 
            // exportToCSVToolStripMenuItem
            // 
            this.exportToCSVToolStripMenuItem.Name = "exportToCSVToolStripMenuItem";
            this.exportToCSVToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.exportToCSVToolStripMenuItem.Text = "Export To CSV ...";
            this.exportToCSVToolStripMenuItem.Click += new System.EventHandler(this.exportToCSVToolStripMenuItem_Click);
            // 
            // ShipBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ShipBrowserControl";
            this.Controls.SetChildIndex(this.scObjectBrowser, 0);
            this.gbDescription.ResumeLayout(false);
            this.gbRequiredSkills.ResumeLayout(false);
            this.scDetailsRight.Panel1.ResumeLayout(false);
            this.scDetailsRight.Panel2.ResumeLayout(false);
            this.scDetailsRight.ResumeLayout(false);
            this.scDetails.Panel1.ResumeLayout(false);
            this.scDetails.Panel2.ResumeLayout(false);
            this.scDetails.ResumeLayout(false);
            this.scObjectBrowser.Panel1.ResumeLayout(false);
            this.scObjectBrowser.Panel2.ResumeLayout(false);
            this.scObjectBrowser.Panel2.PerformLayout();
            this.scObjectBrowser.ResumeLayout(false);
            this.pnlDetails.ResumeLayout(false);
            this.pnlBrowserHeader.ResumeLayout(false);
            this.pnlBrowserHeader.PerformLayout();
            this.scShipSelect.ResumeLayout(false);
            this.gbAttributes.ResumeLayout(false);
            this.ShipPropertiesContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private EVEMon.SkillPlanner.PersistentSplitContainer scShipSelect;
        private System.Windows.Forms.ListView lvShipProperties;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ShipSelectControl shipSelectControl;
        private System.Windows.Forms.GroupBox gbAttributes;
        private System.Windows.Forms.LinkLabel lblBattleclinic;
        private System.Windows.Forms.ContextMenuStrip ShipPropertiesContextMenu;
        private System.Windows.Forms.ToolStripMenuItem exportToCSVToolStripMenuItem;


    }
}
