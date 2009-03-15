namespace EVEMon.SkillPlanner
{
    partial class ItemSelectControl
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
            this.cbDeadspace = new System.Windows.Forms.CheckBox();
            this.cbOfficer = new System.Windows.Forms.CheckBox();
            this.cbFaction = new System.Windows.Forms.CheckBox();
            this.cbTech2 = new System.Windows.Forms.CheckBox();
            this.cbNamed = new System.Windows.Forms.CheckBox();
            this.cbTech1 = new System.Windows.Forms.CheckBox();
            this.cbSlotFilter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gbFittableShipModules = new System.Windows.Forms.GroupBox();
            this.iffsShipFitting = new EVEMon.SkillPlanner.ItemFittingFilterSummary();
            this.cbTech3 = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).BeginInit();
            this.gbFittableShipModules.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbSkillFilter
            // 
            this.cbSkillFilter.Location = new System.Drawing.Point(38, 3);
            this.cbSkillFilter.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.cbSkillFilter.Size = new System.Drawing.Size(145, 21);
            this.cbSkillFilter.TabIndex = 1;
            this.cbSkillFilter.SelectedIndexChanged += new System.EventHandler(this.cbSkillFilter_SelectedIndexChanged);
            // 
            // tbSearchText
            // 
            this.tbSearchText.Location = new System.Drawing.Point(33, 228);
            this.tbSearchText.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.tbSearchText.Size = new System.Drawing.Size(143, 21);
            // 
            // tvItems
            // 
            this.tvItems.LineColor = System.Drawing.Color.Black;
            this.tvItems.Size = new System.Drawing.Size(185, 155);
            this.tvItems.TabIndex = 12;
            // 
            // lbNoMatches
            // 
            this.lbNoMatches.Location = new System.Drawing.Point(2, 2);
            this.lbNoMatches.Size = new System.Drawing.Size(181, 40);
            this.lbNoMatches.TabIndex = 0;
            this.lbNoMatches.Text = "No items match your search.";
            // 
            // lbSearchList
            // 
            this.lbSearchList.Size = new System.Drawing.Size(185, 155);
            this.lbSearchList.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbTech3);
            this.panel1.Controls.Add(this.cbSlotFilter);
            this.panel1.Controls.Add(this.gbFittableShipModules);
            this.panel1.Controls.Add(this.cbDeadspace);
            this.panel1.Controls.Add(this.cbTech1);
            this.panel1.Controls.Add(this.cbOfficer);
            this.panel1.Controls.Add(this.cbTech2);
            this.panel1.Controls.Add(this.cbFaction);
            this.panel1.Controls.Add(this.cbNamed);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Size = new System.Drawing.Size(185, 255);
            this.panel1.Controls.SetChildIndex(this.label2, 0);
            this.panel1.Controls.SetChildIndex(this.tbSearchText, 0);
            this.panel1.Controls.SetChildIndex(this.cbNamed, 0);
            this.panel1.Controls.SetChildIndex(this.cbFaction, 0);
            this.panel1.Controls.SetChildIndex(this.cbTech2, 0);
            this.panel1.Controls.SetChildIndex(this.cbOfficer, 0);
            this.panel1.Controls.SetChildIndex(this.cbTech1, 0);
            this.panel1.Controls.SetChildIndex(this.pbSearchImage, 0);
            this.panel1.Controls.SetChildIndex(this.lbSearchTextHint, 0);
            this.panel1.Controls.SetChildIndex(this.cbDeadspace, 0);
            this.panel1.Controls.SetChildIndex(this.gbFittableShipModules, 0);
            this.panel1.Controls.SetChildIndex(this.label1, 0);
            this.panel1.Controls.SetChildIndex(this.cbSkillFilter, 0);
            this.panel1.Controls.SetChildIndex(this.cbSlotFilter, 0);
            this.panel1.Controls.SetChildIndex(this.cbTech3, 0);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(0, 255);
            this.panel2.Size = new System.Drawing.Size(185, 155);
            this.panel2.TabIndex = 0;
            // 
            // lbSearchTextHint
            // 
            this.lbSearchTextHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lbSearchTextHint.Location = new System.Drawing.Point(34, 229);
            this.lbSearchTextHint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSearchTextHint.Size = new System.Drawing.Size(68, 18);
            this.lbSearchTextHint.TabIndex = 11;
            // 
            // pbSearchImage
            // 
            this.pbSearchImage.Location = new System.Drawing.Point(9, 228);
            // 
            // label1
            // 
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "By Skill:";
            // 
            // cbDeadspace
            // 
            this.cbDeadspace.AutoSize = true;
            this.cbDeadspace.Location = new System.Drawing.Point(90, 102);
            this.cbDeadspace.Name = "cbDeadspace";
            this.cbDeadspace.Size = new System.Drawing.Size(79, 17);
            this.cbDeadspace.TabIndex = 8;
            this.cbDeadspace.Text = "Deadspace";
            this.cbDeadspace.UseVisualStyleBackColor = true;
            this.cbDeadspace.CheckedChanged += new System.EventHandler(this.cbClass_SelectedChanged);
            // 
            // cbOfficer
            // 
            this.cbOfficer.AutoSize = true;
            this.cbOfficer.Location = new System.Drawing.Point(9, 125);
            this.cbOfficer.Name = "cbOfficer";
            this.cbOfficer.Size = new System.Drawing.Size(59, 17);
            this.cbOfficer.TabIndex = 5;
            this.cbOfficer.Text = "Officer";
            this.cbOfficer.UseVisualStyleBackColor = true;
            this.cbOfficer.CheckedChanged += new System.EventHandler(this.cbClass_SelectedChanged);
            // 
            // cbFaction
            // 
            this.cbFaction.AutoSize = true;
            this.cbFaction.Location = new System.Drawing.Point(90, 79);
            this.cbFaction.Name = "cbFaction";
            this.cbFaction.Size = new System.Drawing.Size(61, 17);
            this.cbFaction.TabIndex = 7;
            this.cbFaction.Text = "Faction";
            this.cbFaction.UseVisualStyleBackColor = true;
            this.cbFaction.CheckedChanged += new System.EventHandler(this.cbClass_SelectedChanged);
            // 
            // cbTech2
            // 
            this.cbTech2.AutoSize = true;
            this.cbTech2.Checked = true;
            this.cbTech2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTech2.Location = new System.Drawing.Point(9, 79);
            this.cbTech2.Name = "cbTech2";
            this.cbTech2.Size = new System.Drawing.Size(60, 17);
            this.cbTech2.TabIndex = 4;
            this.cbTech2.Text = "Tech II";
            this.cbTech2.UseVisualStyleBackColor = true;
            this.cbTech2.CheckedChanged += new System.EventHandler(this.cbClass_SelectedChanged);
            // 
            // cbNamed
            // 
            this.cbNamed.AutoSize = true;
            this.cbNamed.Checked = true;
            this.cbNamed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNamed.Location = new System.Drawing.Point(90, 56);
            this.cbNamed.Name = "cbNamed";
            this.cbNamed.Size = new System.Drawing.Size(59, 17);
            this.cbNamed.TabIndex = 6;
            this.cbNamed.Text = "Named";
            this.cbNamed.UseVisualStyleBackColor = true;
            this.cbNamed.CheckedChanged += new System.EventHandler(this.cbClass_SelectedChanged);
            // 
            // cbTech1
            // 
            this.cbTech1.AutoSize = true;
            this.cbTech1.Checked = true;
            this.cbTech1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTech1.Location = new System.Drawing.Point(9, 56);
            this.cbTech1.Name = "cbTech1";
            this.cbTech1.Size = new System.Drawing.Size(56, 17);
            this.cbTech1.TabIndex = 3;
            this.cbTech1.Text = "Tech I";
            this.cbTech1.UseVisualStyleBackColor = true;
            this.cbTech1.CheckedChanged += new System.EventHandler(this.cbClass_SelectedChanged);
            // 
            // cbSlotFilter
            // 
            this.cbSlotFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSlotFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSlotFilter.FormattingEnabled = true;
            this.cbSlotFilter.Items.AddRange(new object[] {
            "All Items",
            "High Slot",
            "Medium Slot",
            "Low Slot",
            "No Slot"});
            this.cbSlotFilter.Location = new System.Drawing.Point(38, 29);
            this.cbSlotFilter.Name = "cbSlotFilter";
            this.cbSlotFilter.Size = new System.Drawing.Size(145, 21);
            this.cbSlotFilter.TabIndex = 2;
            this.cbSlotFilter.SelectedIndexChanged += new System.EventHandler(this.cbSlotFilter_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-1, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "By slot:";
            // 
            // gbFittableShipModules
            // 
            this.gbFittableShipModules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFittableShipModules.Controls.Add(this.iffsShipFitting);
            this.gbFittableShipModules.Location = new System.Drawing.Point(2, 150);
            this.gbFittableShipModules.Name = "gbFittableShipModules";
            this.gbFittableShipModules.Size = new System.Drawing.Size(180, 73);
            this.gbFittableShipModules.TabIndex = 22;
            this.gbFittableShipModules.TabStop = false;
            this.gbFittableShipModules.Text = "By fitting requirements:";
            // 
            // iffsShipFitting
            // 
            this.iffsShipFitting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.iffsShipFitting.Location = new System.Drawing.Point(6, 20);
            this.iffsShipFitting.Name = "iffsShipFitting";
            this.iffsShipFitting.Size = new System.Drawing.Size(168, 46);
            this.iffsShipFitting.TabIndex = 0;
            this.iffsShipFitting.ItemFilterDataChanged += new System.EventHandler<EVEMon.SkillPlanner.ItemFilteringChangedEvent>(this.iffsShipFitting_ItemFilterDataChanged);
            // 
            // cbTech3
            // 
            this.cbTech3.AutoSize = true;
            this.cbTech3.Checked = true;
            this.cbTech3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTech3.Location = new System.Drawing.Point(9, 102);
            this.cbTech3.Name = "cbTech3";
            this.cbTech3.Size = new System.Drawing.Size(64, 17);
            this.cbTech3.TabIndex = 23;
            this.cbTech3.Text = "Tech III";
            this.cbTech3.UseVisualStyleBackColor = true;
            this.cbTech3.CheckedChanged += new System.EventHandler(this.cbClass_SelectedChanged);
            // 
            // ItemSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.Name = "ItemSelectControl";
            this.Size = new System.Drawing.Size(185, 410);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).EndInit();
            this.gbFittableShipModules.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbDeadspace;
        private System.Windows.Forms.CheckBox cbOfficer;
        private System.Windows.Forms.CheckBox cbFaction;
        private System.Windows.Forms.CheckBox cbTech2;
        private System.Windows.Forms.CheckBox cbNamed;
        private System.Windows.Forms.CheckBox cbTech1;
        private System.Windows.Forms.ComboBox cbSlotFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gbFittableShipModules;
        private ItemFittingFilterSummary iffsShipFitting;
        private System.Windows.Forms.CheckBox cbTech3;
    }
}
