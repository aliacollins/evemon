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
            this.components = new System.ComponentModel.Container();
            this.cbSlotFilter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ccbGroupFilter = new EVEMon.Common.Controls.CheckedComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.numPowergrid = new System.Windows.Forms.NumericUpDown();
            this.cbPowergrid = new System.Windows.Forms.CheckBox();
            this.numCPU = new System.Windows.Forms.NumericUpDown();
            this.cbCPU = new System.Windows.Forms.CheckBox();
            this.showAllGroupsCheckbox = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPowergrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCPU)).BeginInit();
            this.SuspendLayout();
            // 
            // cbSkillFilter
            // 
            this.cbSkillFilter.Location = new System.Drawing.Point(45, 3);
            this.cbSkillFilter.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.cbSkillFilter.Size = new System.Drawing.Size(179, 21);
            this.cbSkillFilter.TabIndex = 1;
            this.cbSkillFilter.SelectedIndexChanged += new System.EventHandler(this.cbUsabilityFilter_SelectedIndexChanged);
            // 
            // tbSearchText
            // 
            this.tbSearchText.Location = new System.Drawing.Point(33, 138);
            this.tbSearchText.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.tbSearchText.Size = new System.Drawing.Size(191, 20);
            this.tbSearchText.TabIndex = 10;
            // 
            // tvItems
            // 
            this.tvItems.LineColor = System.Drawing.Color.Black;
            this.tvItems.Size = new System.Drawing.Size(224, 225);
            this.tvItems.TabIndex = 12;
            // 
            // lbNoMatches
            // 
            this.lbNoMatches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lbNoMatches.AutoSize = false;
            this.lbNoMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbNoMatches.Location = new System.Drawing.Point(0, 0);
            this.lbNoMatches.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.lbNoMatches.Size = new System.Drawing.Size(224, 225);
            this.lbNoMatches.TabIndex = 0;
            this.lbNoMatches.Text = "No items match your search.";
            // 
            // lbSearchList
            // 
            this.lbSearchList.Size = new System.Drawing.Size(224, 225);
            this.lbSearchList.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.ccbGroupFilter);
            this.panel1.Controls.Add(this.cbSlotFilter);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Size = new System.Drawing.Size(224, 165);
            this.panel1.TabIndex = 0;
            this.panel1.Controls.SetChildIndex(this.label2, 0);
            this.panel1.Controls.SetChildIndex(this.tbSearchText, 0);
            this.panel1.Controls.SetChildIndex(this.pbSearchImage, 0);
            this.panel1.Controls.SetChildIndex(this.lbSearchTextHint, 0);
            this.panel1.Controls.SetChildIndex(this.label1, 0);
            this.panel1.Controls.SetChildIndex(this.cbSkillFilter, 0);
            this.panel1.Controls.SetChildIndex(this.cbSlotFilter, 0);
            this.panel1.Controls.SetChildIndex(this.ccbGroupFilter, 0);
            this.panel1.Controls.SetChildIndex(this.label4, 0);
            this.panel1.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(0, 165);
            this.panel2.Size = new System.Drawing.Size(224, 225);
            this.panel2.TabIndex = 0;
            // 
            // lbSearchTextHint
            // 
            this.lbSearchTextHint.Location = new System.Drawing.Point(34, 139);
            this.lbSearchTextHint.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbSearchTextHint.Size = new System.Drawing.Size(68, 18);
            this.lbSearchTextHint.TabIndex = 9;
            // 
            // pbSearchImage
            // 
            this.pbSearchImage.Location = new System.Drawing.Point(9, 138);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.TabIndex = 0;
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
            this.cbSlotFilter.Location = new System.Drawing.Point(45, 29);
            this.cbSlotFilter.Name = "cbSlotFilter";
            this.cbSlotFilter.Size = new System.Drawing.Size(179, 21);
            this.cbSlotFilter.TabIndex = 3;
            this.cbSlotFilter.SelectedIndexChanged += new System.EventHandler(this.cbSlotFilter_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Slot:";
            // 
            // ccbGroupFilter
            // 
            this.ccbGroupFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ccbGroupFilter.CheckOnClick = true;
            this.ccbGroupFilter.Cursor = System.Windows.Forms.Cursors.Default;
            this.ccbGroupFilter.CustomTextBuilder = null;
            this.ccbGroupFilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.ccbGroupFilter.DropDownHeight = 1;
            this.ccbGroupFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ccbGroupFilter.FormattingEnabled = true;
            this.ccbGroupFilter.IntegralHeight = false;
            this.ccbGroupFilter.Items.AddRange(new object[] {
            "Named",
            "Faction",
            "Officer",
            "Deadspace"});
            this.ccbGroupFilter.Location = new System.Drawing.Point(45, 56);
            this.ccbGroupFilter.Name = "ccbGroupFilter";
            this.ccbGroupFilter.Size = new System.Drawing.Size(179, 21);
            this.ccbGroupFilter.TabIndex = 7;
            this.ccbGroupFilter.TextForAll = "Any group";
            this.ccbGroupFilter.TextForNone = "Regular items only";
            this.ccbGroupFilter.ToolTip = null;
            this.ccbGroupFilter.ValueSeparator = ", ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Group:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.numPowergrid, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbPowergrid, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.numCPU, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbCPU, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 83);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47.05882F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52.94118F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(214, 48);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // numPowergrid
            // 
            this.numPowergrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numPowergrid.Enabled = false;
            this.numPowergrid.Location = new System.Drawing.Point(85, 25);
            this.numPowergrid.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numPowergrid.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPowergrid.Name = "numPowergrid";
            this.numPowergrid.Size = new System.Drawing.Size(126, 20);
            this.numPowergrid.TabIndex = 3;
            this.numPowergrid.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numPowergrid.ValueChanged += new System.EventHandler(this.numPowergrid_ValueChanged);
            // 
            // cbPowergrid
            // 
            this.cbPowergrid.AutoSize = true;
            this.cbPowergrid.Location = new System.Drawing.Point(3, 25);
            this.cbPowergrid.Name = "cbPowergrid";
            this.cbPowergrid.Size = new System.Drawing.Size(76, 17);
            this.cbPowergrid.TabIndex = 2;
            this.cbPowergrid.Text = "Power limit";
            this.cbPowergrid.UseVisualStyleBackColor = true;
            this.cbPowergrid.CheckedChanged += new System.EventHandler(this.cbPowergrid_CheckedChanged);
            // 
            // numCPU
            // 
            this.numCPU.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numCPU.Enabled = false;
            this.numCPU.Location = new System.Drawing.Point(85, 3);
            this.numCPU.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numCPU.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCPU.Name = "numCPU";
            this.numCPU.Size = new System.Drawing.Size(126, 20);
            this.numCPU.TabIndex = 1;
            this.numCPU.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numCPU.ValueChanged += new System.EventHandler(this.numCPU_ValueChanged);
            // 
            // cbCPU
            // 
            this.cbCPU.AutoSize = true;
            this.cbCPU.Location = new System.Drawing.Point(3, 3);
            this.cbCPU.Name = "cbCPU";
            this.cbCPU.Size = new System.Drawing.Size(68, 16);
            this.cbCPU.TabIndex = 0;
            this.cbCPU.Text = "CPU limit";
            this.cbCPU.UseVisualStyleBackColor = true;
            this.cbCPU.CheckedChanged += new System.EventHandler(this.cbCPU_CheckedChanged);
            // 
            // showAllGroupsCheckbox
            // 
            this.showAllGroupsCheckbox.AutoSize = true;
            this.showAllGroupsCheckbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.showAllGroupsCheckbox.Location = new System.Drawing.Point(0, 390);
            this.showAllGroupsCheckbox.Name = "showAllGroupsCheckbox";
            this.showAllGroupsCheckbox.Padding = new System.Windows.Forms.Padding(5, 3, 3, 0);
            this.showAllGroupsCheckbox.Size = new System.Drawing.Size(224, 20);
            this.showAllGroupsCheckbox.TabIndex = 13;
            this.showAllGroupsCheckbox.Text = "&Show all groups";
            this.showAllGroupsCheckbox.UseVisualStyleBackColor = true;
            this.showAllGroupsCheckbox.CheckedChanged += new System.EventHandler(this.showAllGroupsCheckbox_CheckedChanged);
            // 
            // ItemSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.Controls.Add(this.showAllGroupsCheckbox);
            this.Name = "ItemSelectControl";
            this.Size = new System.Drawing.Size(224, 410);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.showAllGroupsCheckbox, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPowergrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCPU)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ComboBox cbSlotFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private EVEMon.Common.Controls.CheckedComboBox ccbGroupFilter;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbPowergrid;
        private System.Windows.Forms.CheckBox cbCPU;
        private System.Windows.Forms.NumericUpDown numCPU;
        private System.Windows.Forms.NumericUpDown numPowergrid;
        private System.Windows.Forms.CheckBox showAllGroupsCheckbox;
    }
}
