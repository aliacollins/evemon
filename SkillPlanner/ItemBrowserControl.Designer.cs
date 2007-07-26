namespace EVEMon.SkillPlanner
{
    partial class ItemBrowserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemBrowserControl));
            this.splitContainer1 = new EVEMon.SkillPlanner.PersistentSplitContainer();
            this.itemSelectControl1 = new EVEMon.SkillPlanner.ItemSelectControl();
            this.lblHelp = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.splDetails = new EVEMon.Common.SplitContainerMinFixed();
            this.gbAttributes = new System.Windows.Forms.GroupBox();
            this.lvItemProperties = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.splDetailsRight = new EVEMon.Common.SplitContainerMinFixed();
            this.gbDescription = new System.Windows.Forms.GroupBox();
            this.tbDescription = new System.Windows.Forms.TextBox();
            this.gbRequiredSkills = new System.Windows.Forms.GroupBox();
            this.requiredSkillsControl = new EVEMon.SkillPlanner.RequiredSkillsControl();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.eveImage = new EVEMon.Common.EveImage();
            this.lblItemCategory = new System.Windows.Forms.Label();
            this.lblItemName = new System.Windows.Forms.Label();
            this.ttItem = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.splDetails.Panel1.SuspendLayout();
            this.splDetails.Panel2.SuspendLayout();
            this.splDetails.SuspendLayout();
            this.gbAttributes.SuspendLayout();
            this.splDetailsRight.Panel1.SuspendLayout();
            this.splDetailsRight.Panel2.SuspendLayout();
            this.splDetailsRight.SuspendLayout();
            this.gbDescription.SuspendLayout();
            this.gbRequiredSkills.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.itemSelectControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lblHelp);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.RememberDistanceKey = null;
            this.splitContainer1.Size = new System.Drawing.Size(650, 413);
            this.splitContainer1.SplitterDistance = 163;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // itemSelectControl1
            // 
            this.itemSelectControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.itemSelectControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemSelectControl1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemSelectControl1.Location = new System.Drawing.Point(0, 0);
            this.itemSelectControl1.Margin = new System.Windows.Forms.Padding(2);
            this.itemSelectControl1.Name = "itemSelectControl1";
            this.itemSelectControl1.Plan = null;
            this.itemSelectControl1.SelectedObject = null;
            this.itemSelectControl1.SelectedObjects = null;
            this.itemSelectControl1.Size = new System.Drawing.Size(163, 413);
            this.itemSelectControl1.TabIndex = 0;
            this.itemSelectControl1.SelectedObjectChanged += new System.EventHandler<System.EventArgs>(this.itemSelectControl1_SelectedItemChanged);
            // 
            // lblHelp
            // 
            this.lblHelp.AutoSize = true;
            this.lblHelp.Location = new System.Drawing.Point(9, 154);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(378, 65);
            this.lblHelp.TabIndex = 0;
            this.lblHelp.Text = resources.GetString("lblHelp.Text");
            this.lblHelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.splDetails);
            this.panel3.Controls.Add(this.pnlHeader);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(482, 413);
            this.panel3.TabIndex = 14;
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
            this.splDetails.TabIndex = 12;
            // 
            // gbAttributes
            // 
            this.gbAttributes.Controls.Add(this.lvItemProperties);
            this.gbAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAttributes.Location = new System.Drawing.Point(0, 0);
            this.gbAttributes.Name = "gbAttributes";
            this.gbAttributes.Size = new System.Drawing.Size(238, 343);
            this.gbAttributes.TabIndex = 15;
            this.gbAttributes.TabStop = false;
            this.gbAttributes.Text = "Attributes";
            // 
            // lvItemProperties
            // 
            this.lvItemProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvItemProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvItemProperties.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lvItemProperties.FullRowSelect = true;
            this.lvItemProperties.Location = new System.Drawing.Point(3, 16);
            this.lvItemProperties.Name = "lvItemProperties";
            this.lvItemProperties.Size = new System.Drawing.Size(232, 324);
            this.lvItemProperties.TabIndex = 8;
            this.lvItemProperties.UseCompatibleStateImageBehavior = false;
            this.lvItemProperties.View = System.Windows.Forms.View.Details;
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
            this.splDetailsRight.Panel1.Controls.Add(this.gbDescription);
            // 
            // splDetailsRight.Panel2
            // 
            this.splDetailsRight.Panel2.Controls.Add(this.gbRequiredSkills);
            this.splDetailsRight.Panel2MinSize = 120;
            this.splDetailsRight.Size = new System.Drawing.Size(240, 343);
            this.splDetailsRight.SplitterDistance = 218;
            this.splDetailsRight.TabIndex = 0;
            // 
            // gbDescription
            // 
            this.gbDescription.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbDescription.Controls.Add(this.tbDescription);
            this.gbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbDescription.Location = new System.Drawing.Point(0, 0);
            this.gbDescription.Name = "gbDescription";
            this.gbDescription.Size = new System.Drawing.Size(240, 218);
            this.gbDescription.TabIndex = 14;
            this.gbDescription.TabStop = false;
            this.gbDescription.Text = "Description";
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
            this.gbRequiredSkills.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbRequiredSkills.Controls.Add(this.requiredSkillsControl);
            this.gbRequiredSkills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbRequiredSkills.Location = new System.Drawing.Point(0, 0);
            this.gbRequiredSkills.Name = "gbRequiredSkills";
            this.gbRequiredSkills.Size = new System.Drawing.Size(240, 121);
            this.gbRequiredSkills.TabIndex = 13;
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
            this.requiredSkillsControl.TabIndex = 12;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.eveImage);
            this.pnlHeader.Controls.Add(this.lblItemCategory);
            this.pnlHeader.Controls.Add(this.lblItemName);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(482, 70);
            this.pnlHeader.TabIndex = 11;
            // 
            // eveImage
            // 
            this.eveImage.EveItem = null;
            this.eveImage.ImageSize = EVEMon.Common.EveImage.EveImageSize._64_64;
            this.eveImage.Location = new System.Drawing.Point(3, 3);
            this.eveImage.Name = "eveImage";
            this.eveImage.PopUpEnabled = true;
            this.eveImage.Size = new System.Drawing.Size(64, 64);
            this.eveImage.TabIndex = 8;
            // 
            // lblItemCategory
            // 
            this.lblItemCategory.AutoSize = true;
            this.lblItemCategory.Location = new System.Drawing.Point(70, 3);
            this.lblItemCategory.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblItemCategory.Name = "lblItemCategory";
            this.lblItemCategory.Size = new System.Drawing.Size(35, 13);
            this.lblItemCategory.TabIndex = 6;
            this.lblItemCategory.Text = "label2";
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemName.Location = new System.Drawing.Point(70, 16);
            this.lblItemName.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(55, 18);
            this.lblItemName.TabIndex = 7;
            this.lblItemName.Text = "label3";
            // 
            // ItemBrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ItemBrowserControl";
            this.Size = new System.Drawing.Size(650, 413);
            this.VisibleChanged += new System.EventHandler(this.ItemBrowserControl_VisibleChanged);
            this.Load += new System.EventHandler(this.ItemBrowserControl_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.splDetails.Panel1.ResumeLayout(false);
            this.splDetails.Panel2.ResumeLayout(false);
            this.splDetails.ResumeLayout(false);
            this.gbAttributes.ResumeLayout(false);
            this.splDetailsRight.Panel1.ResumeLayout(false);
            this.splDetailsRight.Panel2.ResumeLayout(false);
            this.splDetailsRight.ResumeLayout(false);
            this.gbDescription.ResumeLayout(false);
            this.gbDescription.PerformLayout();
            this.gbRequiredSkills.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private EVEMon.SkillPlanner.PersistentSplitContainer splitContainer1;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblItemCategory;
        private System.Windows.Forms.ListView lvItemProperties;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ItemSelectControl itemSelectControl1;
        private System.Windows.Forms.ToolTip ttItem;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHelp;
        private RequiredSkillsControl requiredSkillsControl;
        private System.Windows.Forms.GroupBox gbRequiredSkills;
        private System.Windows.Forms.GroupBox gbDescription;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox gbAttributes;
        private System.Windows.Forms.TextBox tbDescription;
        private EVEMon.Common.SplitContainerMinFixed splDetails;
        private EVEMon.Common.SplitContainerMinFixed splDetailsRight;
        private EVEMon.Common.EveImage eveImage;
    }
}
