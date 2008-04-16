namespace EVEMon.SkillPlanner
{
    partial class ItemFittingFilterDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemFittingFilterDialog));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cbHighSlot = new System.Windows.Forms.CheckBox();
            this.cbMedSlot = new System.Windows.Forms.CheckBox();
            this.cbLowSlot = new System.Windows.Forms.CheckBox();
            this.udCpuAvailable = new System.Windows.Forms.NumericUpDown();
            this.udPowergridAvailable = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udCpuAvailable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udPowergridAvailable)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.udCpuAvailable, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.udPowergridAvailable, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 0, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(252, 163);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.cbHighSlot);
            this.flowLayoutPanel1.Controls.Add(this.cbMedSlot);
            this.flowLayoutPanel1.Controls.Add(this.cbLowSlot);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(129, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(120, 70);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // cbHighSlot
            // 
            this.cbHighSlot.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbHighSlot.AutoSize = true;
            this.cbHighSlot.Checked = true;
            this.cbHighSlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHighSlot.Location = new System.Drawing.Point(3, 3);
            this.cbHighSlot.Name = "cbHighSlot";
            this.cbHighSlot.Size = new System.Drawing.Size(67, 17);
            this.cbHighSlot.TabIndex = 0;
            this.cbHighSlot.Text = "&High slot";
            this.cbHighSlot.UseVisualStyleBackColor = true;
            // 
            // cbMedSlot
            // 
            this.cbMedSlot.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbMedSlot.AutoSize = true;
            this.cbMedSlot.Checked = true;
            this.cbMedSlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMedSlot.Location = new System.Drawing.Point(3, 26);
            this.cbMedSlot.Name = "cbMedSlot";
            this.cbMedSlot.Size = new System.Drawing.Size(82, 17);
            this.cbMedSlot.TabIndex = 1;
            this.cbMedSlot.Text = "&Medium slot";
            this.cbMedSlot.UseVisualStyleBackColor = true;
            // 
            // cbLowSlot
            // 
            this.cbLowSlot.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbLowSlot.AutoSize = true;
            this.cbLowSlot.Checked = true;
            this.cbLowSlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLowSlot.Location = new System.Drawing.Point(3, 49);
            this.cbLowSlot.Name = "cbLowSlot";
            this.cbLowSlot.Size = new System.Drawing.Size(65, 17);
            this.cbLowSlot.TabIndex = 2;
            this.cbLowSlot.Text = "&Low slot";
            this.cbLowSlot.UseVisualStyleBackColor = true;
            // 
            // udCpuAvailable
            // 
            this.udCpuAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.udCpuAvailable.ForeColor = System.Drawing.SystemColors.ControlText;
            this.udCpuAvailable.Location = new System.Drawing.Point(129, 79);
            this.udCpuAvailable.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.udCpuAvailable.Name = "udCpuAvailable";
            this.udCpuAvailable.Size = new System.Drawing.Size(120, 20);
            this.udCpuAvailable.TabIndex = 1;
            this.udCpuAvailable.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udCpuAvailable.ThousandsSeparator = true;
            // 
            // udPowergridAvailable
            // 
            this.udPowergridAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.udPowergridAvailable.ForeColor = System.Drawing.SystemColors.ControlText;
            this.udPowergridAvailable.Location = new System.Drawing.Point(129, 105);
            this.udPowergridAvailable.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.udPowergridAvailable.Name = "udPowergridAvailable";
            this.udPowergridAvailable.Size = new System.Drawing.Size(120, 20);
            this.udPowergridAvailable.TabIndex = 2;
            this.udPowergridAvailable.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udPowergridAvailable.ThousandsSeparator = true;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Powergrid available";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "CPU available";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Slot type";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel2, 2);
            this.flowLayoutPanel2.Controls.Add(this.btnCancel);
            this.flowLayoutPanel2.Controls.Add(this.btnReset);
            this.flowLayoutPanel2.Controls.Add(this.btnOk);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 131);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(246, 29);
            this.flowLayoutPanel2.TabIndex = 8;
            this.flowLayoutPanel2.WrapContents = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(84, 3);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(165, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // ItemFittingFilterDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(249, 161);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ItemFittingFilterDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filter by items I can fit";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udCpuAvailable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udPowergridAvailable)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox cbHighSlot;
        private System.Windows.Forms.CheckBox cbMedSlot;
        private System.Windows.Forms.CheckBox cbLowSlot;
        private System.Windows.Forms.NumericUpDown udCpuAvailable;
        private System.Windows.Forms.NumericUpDown udPowergridAvailable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnCancel;
    }
}
