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
            this.lblBattleclinic = new System.Windows.Forms.LinkLabel();
            this.gbAttributes = new System.Windows.Forms.GroupBox();
            this.lvShipProperties = new System.Windows.Forms.ListView();
            this.chAttribute = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ShipPropertiesContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shipSelectControl = new EVEMon.SkillPlanner.ShipSelectControl();
            this.scDetailsLowerRight = new System.Windows.Forms.SplitContainer();
            this.gbRequiredSkills = new System.Windows.Forms.GroupBox();
            this.requiredSkillsControl = new EVEMon.SkillPlanner.RequiredSkillsControl();
            ((System.ComponentModel.ISupportInitialize)(this.scObjectBrowser)).BeginInit();
            this.scObjectBrowser.Panel1.SuspendLayout();
            this.scObjectBrowser.Panel2.SuspendLayout();
            this.scObjectBrowser.SuspendLayout();
            this.pnlDetails.SuspendLayout();
            this.pnlBrowserHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDetailsRight)).BeginInit();
            this.scDetailsRight.Panel1.SuspendLayout();
            this.scDetailsRight.Panel2.SuspendLayout();
            this.scDetailsRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDetails)).BeginInit();
            this.scDetails.Panel1.SuspendLayout();
            this.scDetails.Panel2.SuspendLayout();
            this.scDetails.SuspendLayout();
            this.gbDescription.SuspendLayout();
            this.gbAttributes.SuspendLayout();
            this.ShipPropertiesContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scDetailsLowerRight)).BeginInit();
            this.scDetailsLowerRight.Panel2.SuspendLayout();
            this.scDetailsLowerRight.SuspendLayout();
            this.gbRequiredSkills.SuspendLayout();
            this.SuspendLayout();
            // 
            // scObjectBrowser
            // 
            // 
            // scObjectBrowser.Panel1
            // 
            this.scObjectBrowser.Panel1.Controls.Add(this.shipSelectControl);
            // 
            // pnlBrowserHeader
            // 
            this.pnlBrowserHeader.Controls.Add(this.lblBattleclinic);
            this.pnlBrowserHeader.Controls.SetChildIndex(this.lblEveObjName, 0);
            this.pnlBrowserHeader.Controls.SetChildIndex(this.lblBattleclinic, 0);
            this.pnlBrowserHeader.Controls.SetChildIndex(this.lblEveObjCategory, 0);
            this.pnlBrowserHeader.Controls.SetChildIndex(this.eoImage, 0);
            // 
            // scDetailsRight
            // 
            // 
            // scDetailsRight.Panel2
            // 
            this.scDetailsRight.Panel2.Controls.Add(this.scDetailsLowerRight);
            this.scDetailsRight.Panel2MinSize = 108;
            this.scDetailsRight.SplitterDistance = 231;
            // 
            // scDetails
            // 
            // 
            // scDetails.Panel1
            // 
            this.scDetails.Panel1.Controls.Add(this.gbAttributes);
            // 
            // gbDescription
            // 
            this.gbDescription.Size = new System.Drawing.Size(240, 231);
            // 
            // tbDescription
            // 
            this.tbDescription.Size = new System.Drawing.Size(234, 212);
            // 
            // lblBattleclinic
            // 
            this.lblBattleclinic.AutoSize = true;
            this.lblBattleclinic.Location = new System.Drawing.Point(70, 54);
            this.lblBattleclinic.Name = "lblBattleclinic";
            this.lblBattleclinic.Size = new System.Drawing.Size(102, 13);
            this.lblBattleclinic.TabIndex = 11;
            this.lblBattleclinic.TabStop = true;
            this.lblBattleclinic.Text = "BattleClinic loadouts";
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
            this.chAttribute,
            this.chValue});
            this.lvShipProperties.ContextMenuStrip = this.ShipPropertiesContextMenu;
            this.lvShipProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvShipProperties.FullRowSelect = true;
            this.lvShipProperties.HideSelection = false;
            this.lvShipProperties.Location = new System.Drawing.Point(3, 16);
            this.lvShipProperties.Name = "lvShipProperties";
            this.lvShipProperties.Size = new System.Drawing.Size(232, 324);
            this.lvShipProperties.TabIndex = 3;
            this.lvShipProperties.UseCompatibleStateImageBehavior = false;
            this.lvShipProperties.View = System.Windows.Forms.View.Details;
            // 
            // chAttribute
            // 
            this.chAttribute.Text = "Attribute";
            this.chAttribute.Width = 120;
            // 
            // chValue
            // 
            this.chValue.Text = "Value";
            this.chValue.Width = 120;
            // 
            // ShipPropertiesContextMenu
            // 
            this.ShipPropertiesContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToCSVToolStripMenuItem});
            this.ShipPropertiesContextMenu.Name = "ShipPropertiesContextMenu";
            this.ShipPropertiesContextMenu.Size = new System.Drawing.Size(158, 26);
            // 
            // exportToCSVToolStripMenuItem
            // 
            this.exportToCSVToolStripMenuItem.Name = "exportToCSVToolStripMenuItem";
            this.exportToCSVToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.exportToCSVToolStripMenuItem.Text = "Export To CSV...";
            this.exportToCSVToolStripMenuItem.Click += new System.EventHandler(this.exportToCSVToolStripMenuItem_Click);
            // 
            // shipSelectControl
            // 
            this.shipSelectControl.AutoSize = true;
            this.shipSelectControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.shipSelectControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shipSelectControl.Location = new System.Drawing.Point(0, 0);
            this.shipSelectControl.Margin = new System.Windows.Forms.Padding(2);
            this.shipSelectControl.Name = "shipSelectControl";
            this.shipSelectControl.Plan = null;
            this.shipSelectControl.SelectedObject = null;
            this.shipSelectControl.Size = new System.Drawing.Size(163, 413);
            this.shipSelectControl.TabIndex = 0;
            // 
            // scDetailsLowerRight
            // 
            this.scDetailsLowerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scDetailsLowerRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scDetailsLowerRight.Location = new System.Drawing.Point(0, 0);
            this.scDetailsLowerRight.Name = "scDetailsLowerRight";
            this.scDetailsLowerRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.scDetailsLowerRight.Panel1Collapsed = true;
            this.scDetailsLowerRight.Panel1MinSize = 0;
            // 
            // scDetailsLowerRight.Panel2
            // 
            this.scDetailsLowerRight.Panel2.Controls.Add(this.gbRequiredSkills);
            this.scDetailsLowerRight.Panel2MinSize = 104;
            this.scDetailsLowerRight.Size = new System.Drawing.Size(240, 108);
            this.scDetailsLowerRight.SplitterDistance = 0;
            this.scDetailsLowerRight.TabIndex = 0;
            // 
            // gbRequiredSkills
            // 
            this.gbRequiredSkills.Controls.Add(this.requiredSkillsControl);
            this.gbRequiredSkills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRequiredSkills.Location = new System.Drawing.Point(0, 0);
            this.gbRequiredSkills.Name = "gbRequiredSkills";
            this.gbRequiredSkills.Size = new System.Drawing.Size(240, 108);
            this.gbRequiredSkills.TabIndex = 0;
            this.gbRequiredSkills.TabStop = false;
            this.gbRequiredSkills.Text = "Required Skills";
            // 
            // requiredSkillsControl
            // 
            this.requiredSkillsControl.Activity = EVEMon.Common.BlueprintActivity.None;
            this.requiredSkillsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.requiredSkillsControl.Location = new System.Drawing.Point(3, 16);
            this.requiredSkillsControl.MinimumSize = new System.Drawing.Size(187, 0);
            this.requiredSkillsControl.Name = "requiredSkillsControl";
            this.requiredSkillsControl.Object = null;
            this.requiredSkillsControl.Plan = null;
            this.requiredSkillsControl.Size = new System.Drawing.Size(234, 89);
            this.requiredSkillsControl.TabIndex = 0;
            // 
            // ShipBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ShipBrowserControl";
            this.Controls.SetChildIndex(this.scObjectBrowser, 0);
            this.scObjectBrowser.Panel1.ResumeLayout(false);
            this.scObjectBrowser.Panel1.PerformLayout();
            this.scObjectBrowser.Panel2.ResumeLayout(false);
            this.scObjectBrowser.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scObjectBrowser)).EndInit();
            this.scObjectBrowser.ResumeLayout(false);
            this.pnlDetails.ResumeLayout(false);
            this.pnlBrowserHeader.ResumeLayout(false);
            this.pnlBrowserHeader.PerformLayout();
            this.scDetailsRight.Panel1.ResumeLayout(false);
            this.scDetailsRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDetailsRight)).EndInit();
            this.scDetailsRight.ResumeLayout(false);
            this.scDetails.Panel1.ResumeLayout(false);
            this.scDetails.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDetails)).EndInit();
            this.scDetails.ResumeLayout(false);
            this.gbDescription.ResumeLayout(false);
            this.gbAttributes.ResumeLayout(false);
            this.ShipPropertiesContextMenu.ResumeLayout(false);
            this.scDetailsLowerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scDetailsLowerRight)).EndInit();
            this.scDetailsLowerRight.ResumeLayout(false);
            this.gbRequiredSkills.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvShipProperties;
        private System.Windows.Forms.ColumnHeader chAttribute;
        private System.Windows.Forms.ColumnHeader chValue;
        private ShipSelectControl shipSelectControl;
        private System.Windows.Forms.GroupBox gbAttributes;
        private System.Windows.Forms.LinkLabel lblBattleclinic;
        private System.Windows.Forms.ContextMenuStrip ShipPropertiesContextMenu;
        private System.Windows.Forms.ToolStripMenuItem exportToCSVToolStripMenuItem;
        private System.Windows.Forms.SplitContainer scDetailsLowerRight;
        private System.Windows.Forms.GroupBox gbRequiredSkills;
        private RequiredSkillsControl requiredSkillsControl;

    }
}
