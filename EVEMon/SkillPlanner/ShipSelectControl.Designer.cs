namespace EVEMon.SkillPlanner
{
    partial class ShipSelectControl
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
        private void InitializeComponent()
        {
            this.cbCaldari = new System.Windows.Forms.CheckBox();
            this.cbFaction = new System.Windows.Forms.CheckBox();
            this.cbGallente = new System.Windows.Forms.CheckBox();
            this.cbMinmatar = new System.Windows.Forms.CheckBox();
            this.cbORE = new System.Windows.Forms.CheckBox();
            this.cbAmarr = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbSkillFilter
            // 
            this.cbSkillFilter.Location = new System.Drawing.Point(40, 3);
            this.cbSkillFilter.Size = new System.Drawing.Size(189, 21);
            this.cbSkillFilter.SelectedIndexChanged += new System.EventHandler(this.cbSkillFilter_SelectedIndexChanged);
            // 
            // tbSearchText
            // 
            this.tbSearchText.Location = new System.Drawing.Point(31, 132);
            this.tbSearchText.Margin = new System.Windows.Forms.Padding(0);
            this.tbSearchText.Size = new System.Drawing.Size(198, 20);
            this.tbSearchText.TabIndex = 35;
            // 
            // tvItems
            // 
            this.tvItems.LineColor = System.Drawing.Color.Black;
            this.tvItems.Margin = new System.Windows.Forms.Padding(0);
            this.tvItems.Size = new System.Drawing.Size(229, 319);
            // 
            // lbNoMatches
            // 
            this.lbNoMatches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lbNoMatches.AutoSize = false;
            this.lbNoMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbNoMatches.Location = new System.Drawing.Point(0, 0);
            this.lbNoMatches.Margin = new System.Windows.Forms.Padding(0);
            this.lbNoMatches.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
            this.lbNoMatches.Size = new System.Drawing.Size(229, 319);
            // 
            // lbSearchList
            // 
            this.lbSearchList.Margin = new System.Windows.Forms.Padding(0);
            this.lbSearchList.Size = new System.Drawing.Size(229, 319);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Size = new System.Drawing.Size(229, 159);
            this.panel1.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            this.panel1.Controls.SetChildIndex(this.pbSearchImage, 0);
            this.panel1.Controls.SetChildIndex(this.tbSearchText, 0);
            this.panel1.Controls.SetChildIndex(this.lbSearchTextHint, 0);
            this.panel1.Controls.SetChildIndex(this.label1, 0);
            this.panel1.Controls.SetChildIndex(this.cbSkillFilter, 0);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(0, 159);
            this.panel2.Size = new System.Drawing.Size(229, 319);
            // 
            // lbSearchTextHint
            // 
            this.lbSearchTextHint.Location = new System.Drawing.Point(32, 133);
            this.lbSearchTextHint.Margin = new System.Windows.Forms.Padding(0);
            this.lbSearchTextHint.Size = new System.Drawing.Size(76, 18);
            // 
            // pbSearchImage
            // 
            this.pbSearchImage.Location = new System.Drawing.Point(6, 132);
            this.pbSearchImage.Margin = new System.Windows.Forms.Padding(0);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 6);
            // 
            // cbCaldari
            // 
            this.cbCaldari.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.cbCaldari.AutoSize = true;
            this.cbCaldari.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbCaldari.Location = new System.Drawing.Point(0, 25);
            this.cbCaldari.Margin = new System.Windows.Forms.Padding(0);
            this.cbCaldari.Name = "cbCaldari";
            this.cbCaldari.Size = new System.Drawing.Size(58, 17);
            this.cbCaldari.TabIndex = 27;
            this.cbCaldari.Text = "Caldari";
            this.cbCaldari.UseVisualStyleBackColor = true;
            this.cbCaldari.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            // 
            // cbFaction
            // 
            this.cbFaction.AutoSize = true;
            this.cbFaction.Location = new System.Drawing.Point(108, 25);
            this.cbFaction.Margin = new System.Windows.Forms.Padding(0);
            this.cbFaction.Name = "cbFaction";
            this.cbFaction.Size = new System.Drawing.Size(61, 17);
            this.cbFaction.TabIndex = 30;
            this.cbFaction.Text = "Faction";
            this.cbFaction.UseVisualStyleBackColor = true;
            this.cbFaction.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            // 
            // cbGallente
            // 
            this.cbGallente.AutoSize = true;
            this.cbGallente.Location = new System.Drawing.Point(0, 50);
            this.cbGallente.Margin = new System.Windows.Forms.Padding(0);
            this.cbGallente.Name = "cbGallente";
            this.cbGallente.Size = new System.Drawing.Size(65, 17);
            this.cbGallente.TabIndex = 28;
            this.cbGallente.Text = "Gallente";
            this.cbGallente.UseVisualStyleBackColor = true;
            this.cbGallente.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            // 
            // cbMinmatar
            // 
            this.cbMinmatar.AutoSize = true;
            this.cbMinmatar.Location = new System.Drawing.Point(108, 0);
            this.cbMinmatar.Margin = new System.Windows.Forms.Padding(0);
            this.cbMinmatar.Name = "cbMinmatar";
            this.cbMinmatar.Size = new System.Drawing.Size(69, 17);
            this.cbMinmatar.TabIndex = 29;
            this.cbMinmatar.Text = "Minmatar";
            this.cbMinmatar.UseVisualStyleBackColor = true;
            this.cbMinmatar.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            // 
            // cbORE
            // 
            this.cbORE.AutoSize = true;
            this.cbORE.Location = new System.Drawing.Point(108, 50);
            this.cbORE.Margin = new System.Windows.Forms.Padding(0);
            this.cbORE.Name = "cbORE";
            this.cbORE.Size = new System.Drawing.Size(49, 17);
            this.cbORE.TabIndex = 31;
            this.cbORE.Text = "ORE";
            this.cbORE.UseVisualStyleBackColor = true;
            this.cbORE.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            // 
            // cbAmarr
            // 
            this.cbAmarr.AutoSize = true;
            this.cbAmarr.Location = new System.Drawing.Point(0, 0);
            this.cbAmarr.Margin = new System.Windows.Forms.Padding(0);
            this.cbAmarr.Name = "cbAmarr";
            this.cbAmarr.Size = new System.Drawing.Size(53, 17);
            this.cbAmarr.TabIndex = 26;
            this.cbAmarr.Text = "Amarr";
            this.cbAmarr.UseVisualStyleBackColor = true;
            this.cbAmarr.CheckedChanged += new System.EventHandler(this.cbRace_SelectedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.cbAmarr, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbMinmatar, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbCaldari, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbFaction, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbGallente, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.cbORE, 1, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 39);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(217, 77);
            this.tableLayoutPanel1.TabIndex = 36;
            // 
            // ShipSelectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ShipSelectControl";
            this.Size = new System.Drawing.Size(229, 478);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSearchImage)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.CheckBox cbFaction;
        private System.Windows.Forms.CheckBox cbAmarr;
        private System.Windows.Forms.CheckBox cbORE;
        private System.Windows.Forms.CheckBox cbGallente;
        private System.Windows.Forms.CheckBox cbMinmatar;
        private System.Windows.Forms.CheckBox cbCaldari;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
