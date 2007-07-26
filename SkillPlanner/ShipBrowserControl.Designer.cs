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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShipBrowserControl));
            this.scShipSelect = new EVEMon.SkillPlanner.PersistentSplitContainer();
            this.shipSelectControl = new EVEMon.SkillPlanner.ShipSelectControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splDetails = new EVEMon.Common.SplitContainerMinFixed();
            this.gbAttributes = new System.Windows.Forms.GroupBox();
            this.lvShipProperties = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.splDetailsRight = new EVEMon.Common.SplitContainerMinFixed();
            this.gbDesription = new System.Windows.Forms.GroupBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.gbRequiredSkills = new System.Windows.Forms.GroupBox();
            this.requiredSkillsControl = new EVEMon.SkillPlanner.RequiredSkillsControl();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblBattleclinic = new System.Windows.Forms.LinkLabel();
            this.eveImage = new EVEMon.Common.EveImage();
            this.lblShipName = new System.Windows.Forms.Label();
            this.lblShipClass = new System.Windows.Forms.Label();
            this.lblHelp = new System.Windows.Forms.Label();
            this.scShipSelect.Panel1.SuspendLayout();
            this.scShipSelect.Panel2.SuspendLayout();
            this.scShipSelect.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splDetails.Panel1.SuspendLayout();
            this.splDetails.Panel2.SuspendLayout();
            this.splDetails.SuspendLayout();
            this.gbAttributes.SuspendLayout();
            this.splDetailsRight.Panel1.SuspendLayout();
            this.splDetailsRight.Panel2.SuspendLayout();
            this.splDetailsRight.SuspendLayout();
            this.gbDesription.SuspendLayout();
            this.gbRequiredSkills.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // scShipSelect
            // 
            this.scShipSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scShipSelect.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scShipSelect.Location = new System.Drawing.Point(0, 0);
            this.scShipSelect.Margin = new System.Windows.Forms.Padding(2);
            this.scShipSelect.Name = "scShipSelect";
            // 
            // scShipSelect.Panel1
            // 
            this.scShipSelect.Panel1.Controls.Add(this.shipSelectControl);
            // 
            // scShipSelect.Panel2
            // 
            this.scShipSelect.Panel2.Controls.Add(this.panel1);
            this.scShipSelect.Panel2.Controls.Add(this.lblHelp);
            this.scShipSelect.RememberDistanceKey = null;
            this.scShipSelect.Size = new System.Drawing.Size(650, 413);
            this.scShipSelect.SplitterDistance = 163;
            this.scShipSelect.SplitterWidth = 5;
            this.scShipSelect.TabIndex = 1;
            // 
            // shipSelectControl
            // 
            this.shipSelectControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.shipSelectControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shipSelectControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.shipSelectControl.Location = new System.Drawing.Point(0, 0);
            this.shipSelectControl.Margin = new System.Windows.Forms.Padding(2);
            this.shipSelectControl.Name = "shipSelectControl";
            this.shipSelectControl.Plan = null;
            this.shipSelectControl.SelectedObject = null;
            this.shipSelectControl.SelectedObjects = null;
            this.shipSelectControl.Size = new System.Drawing.Size(163, 413);
            this.shipSelectControl.TabIndex = 0;
            this.shipSelectControl.SelectedObjectChanged += new System.EventHandler<System.EventArgs>(this.shipSelectControl_SelectedShipChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splDetails);
            this.panel1.Controls.Add(this.pnlHeader);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(482, 413);
            this.panel1.TabIndex = 3;
            // 
            // splDetails
            // 
            this.splDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splDetails.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splDetails.Location = new System.Drawing.Point(0, 70);
            this.splDetails.Name = "splDetails";
            // 
            // splDetails.Panel1
            // 
            this.splDetails.Panel1.Controls.Add(this.gbAttributes);
            // 
            // splDetails.Panel2
            // 
            this.splDetails.Panel2.Controls.Add(this.splDetailsRight);
            this.splDetails.Panel2MinSize = 240;
            this.splDetails.Size = new System.Drawing.Size(482, 343);
            this.splDetails.SplitterDistance = 238;
            this.splDetails.TabIndex = 3;
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
            // splDetailsRight
            // 
            this.splDetailsRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splDetailsRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splDetailsRight.Location = new System.Drawing.Point(0, 0);
            this.splDetailsRight.Name = "splDetailsRight";
            this.splDetailsRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splDetailsRight.Panel1
            // 
            this.splDetailsRight.Panel1.Controls.Add(this.gbDesription);
            // 
            // splDetailsRight.Panel2
            // 
            this.splDetailsRight.Panel2.Controls.Add(this.gbRequiredSkills);
            this.splDetailsRight.Panel2MinSize = 120;
            this.splDetailsRight.Size = new System.Drawing.Size(240, 343);
            this.splDetailsRight.SplitterDistance = 218;
            this.splDetailsRight.TabIndex = 0;
            // 
            // gbDesription
            // 
            this.gbDesription.Controls.Add(this.tbDescription);
            this.gbDesription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbDesription.Location = new System.Drawing.Point(0, 0);
            this.gbDesription.Name = "gbDesription";
            this.gbDesription.Size = new System.Drawing.Size(240, 218);
            this.gbDesription.TabIndex = 1;
            this.gbDesription.TabStop = false;
            this.gbDesription.Text = "Description";
            // 
            // tbDescription
            // 
            this.tbDescription.BackColor = System.Drawing.SystemColors.Window;
            this.tbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbDescription.Location = new System.Drawing.Point(3, 16);
            this.tbDescription.Multiline = true;
            this.tbDescription.Name = "tbDescription";
            this.tbDescription.ReadOnly = true;
            this.tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDescription.Size = new System.Drawing.Size(234, 199);
            this.tbDescription.TabIndex = 0;
            // 
            // gbRequiredSkills
            // 
            this.gbRequiredSkills.Controls.Add(this.requiredSkillsControl);
            this.gbRequiredSkills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRequiredSkills.Location = new System.Drawing.Point(0, 0);
            this.gbRequiredSkills.Margin = new System.Windows.Forms.Padding(6);
            this.gbRequiredSkills.Name = "gbRequiredSkills";
            this.gbRequiredSkills.Size = new System.Drawing.Size(240, 121);
            this.gbRequiredSkills.TabIndex = 5;
            this.gbRequiredSkills.TabStop = false;
            this.gbRequiredSkills.Text = "Required Skills";
            // 
            // requiredSkillsControl
            // 
            this.requiredSkillsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.requiredSkillsControl.EveItem = null;
            this.requiredSkillsControl.Location = new System.Drawing.Point(3, 16);
            this.requiredSkillsControl.Name = "requiredSkillsControl";
            this.requiredSkillsControl.Plan = null;
            this.requiredSkillsControl.Size = new System.Drawing.Size(234, 102);
            this.requiredSkillsControl.TabIndex = 0;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.lblBattleclinic);
            this.pnlHeader.Controls.Add(this.eveImage);
            this.pnlHeader.Controls.Add(this.lblShipName);
            this.pnlHeader.Controls.Add(this.lblShipClass);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(482, 70);
            this.pnlHeader.TabIndex = 4;
            // 
            // lblBattleclinic
            // 
            this.lblBattleclinic.AutoSize = true;
            this.lblBattleclinic.Location = new System.Drawing.Point(74, 54);
            this.lblBattleclinic.Name = "lblBattleclinic";
            this.lblBattleclinic.Size = new System.Drawing.Size(101, 13);
            this.lblBattleclinic.TabIndex = 11;
            this.lblBattleclinic.TabStop = true;
            this.lblBattleclinic.Text = "Battleclinic loadouts";
            this.lblBattleclinic.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblBattleclinic_LinkClicked);
            // 
            // eveImage
            // 
            this.eveImage.EveItem = null;
            this.eveImage.ImageSize = EVEMon.Common.EveImage.EveImageSize._64_64;
            this.eveImage.Location = new System.Drawing.Point(4, 3);
            this.eveImage.Name = "eveImage";
            this.eveImage.PopUpEnabled = true;
            this.eveImage.Size = new System.Drawing.Size(64, 64);
            this.eveImage.TabIndex = 3;
            // 
            // lblShipName
            // 
            this.lblShipName.AutoSize = true;
            this.lblShipName.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShipName.Location = new System.Drawing.Point(74, 16);
            this.lblShipName.Name = "lblShipName";
            this.lblShipName.Size = new System.Drawing.Size(88, 18);
            this.lblShipName.TabIndex = 2;
            this.lblShipName.Text = "Ship Name";
            // 
            // lblShipClass
            // 
            this.lblShipClass.AutoSize = true;
            this.lblShipClass.Location = new System.Drawing.Point(74, 3);
            this.lblShipClass.Name = "lblShipClass";
            this.lblShipClass.Size = new System.Drawing.Size(65, 13);
            this.lblShipClass.TabIndex = 1;
            this.lblShipClass.Text = "Ship > Class";
            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = true;
            this.lblHelp.Location = new System.Drawing.Point(9, 154);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(378, 65);
            this.lblHelp.TabIndex = 2;
            this.lblHelp.Text = resources.GetString("lblHelp.Text");
            this.lblHelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShipBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scShipSelect);
            this.Name = "ShipBrowserControl";
            this.Size = new System.Drawing.Size(650, 413);
            this.VisibleChanged += new System.EventHandler(this.ShipBrowserControl_VisibleChanged);
            this.scShipSelect.Panel1.ResumeLayout(false);
            this.scShipSelect.Panel2.ResumeLayout(false);
            this.scShipSelect.Panel2.PerformLayout();
            this.scShipSelect.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.splDetails.Panel1.ResumeLayout(false);
            this.splDetails.Panel2.ResumeLayout(false);
            this.splDetails.ResumeLayout(false);
            this.gbAttributes.ResumeLayout(false);
            this.splDetailsRight.Panel1.ResumeLayout(false);
            this.splDetailsRight.Panel2.ResumeLayout(false);
            this.splDetailsRight.ResumeLayout(false);
            this.gbDesription.ResumeLayout(false);
            this.gbDesription.PerformLayout();
            this.gbRequiredSkills.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private EVEMon.SkillPlanner.PersistentSplitContainer scShipSelect;
        private System.Windows.Forms.GroupBox gbRequiredSkills;
        private System.Windows.Forms.Label lblShipName;
        private System.Windows.Forms.Label lblShipClass;
        private System.Windows.Forms.ListView lvShipProperties;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ShipSelectControl shipSelectControl;
        private RequiredSkillsControl requiredSkillsControl;
        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.Panel pnlHeader;
        private EVEMon.Common.SplitContainerMinFixed splDetails;
        private System.Windows.Forms.Panel panel1;
        private EVEMon.Common.SplitContainerMinFixed splDetailsRight;
        private System.Windows.Forms.GroupBox gbDesription;
        private System.Windows.Forms.TextBox tbDescription;
        private System.Windows.Forms.GroupBox gbAttributes;
        private EVEMon.Common.EveImage eveImage;
        private System.Windows.Forms.LinkLabel lblBattleclinic;


    }
}
