namespace EVEMon.SkillPlanner
{
    partial class AttributesOptimizationSettingsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributesOptimizationSettingsForm));
            this.radioTrainedSKills = new System.Windows.Forms.RadioButton();
            this.radioWholePlan = new System.Windows.Forms.RadioButton();
            this.radioPartialPlan = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboStartSkill = new System.Windows.Forms.ComboBox();
            this.numYears = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numMonths = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupPartialPlan = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numYears)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonths)).BeginInit();
            this.groupPartialPlan.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioTrainedSKills
            // 
            this.radioTrainedSKills.AutoSize = true;
            this.radioTrainedSKills.Location = new System.Drawing.Point(26, 38);
            this.radioTrainedSKills.Name = "radioTrainedSKills";
            this.radioTrainedSKills.Size = new System.Drawing.Size(134, 17);
            this.radioTrainedSKills.TabIndex = 1;
            this.radioTrainedSKills.Text = "Skills already trained (*)";
            this.radioTrainedSKills.UseVisualStyleBackColor = true;
            // 
            // radioWholePlan
            // 
            this.radioWholePlan.AutoSize = true;
            this.radioWholePlan.Checked = true;
            this.radioWholePlan.Location = new System.Drawing.Point(26, 62);
            this.radioWholePlan.Name = "radioWholePlan";
            this.radioWholePlan.Size = new System.Drawing.Size(115, 17);
            this.radioWholePlan.TabIndex = 2;
            this.radioWholePlan.TabStop = true;
            this.radioWholePlan.Text = "Whole current plan";
            this.radioWholePlan.UseVisualStyleBackColor = true;
            // 
            // radioPartialPlan
            // 
            this.radioPartialPlan.AutoSize = true;
            this.radioPartialPlan.Location = new System.Drawing.Point(26, 86);
            this.radioPartialPlan.Name = "radioPartialPlan";
            this.radioPartialPlan.Size = new System.Drawing.Size(133, 17);
            this.radioPartialPlan.TabIndex = 3;
            this.radioPartialPlan.Text = "Part of the current plan";
            this.radioPartialPlan.UseVisualStyleBackColor = true;
            this.radioPartialPlan.CheckedChanged += new System.EventHandler(this.radioPartialPlan_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Base optimization on...";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Start at";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Stop after";
            // 
            // comboStartSkill
            // 
            this.comboStartSkill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboStartSkill.FormattingEnabled = true;
            this.comboStartSkill.Location = new System.Drawing.Point(56, 19);
            this.comboStartSkill.Name = "comboStartSkill";
            this.comboStartSkill.Size = new System.Drawing.Size(204, 21);
            this.comboStartSkill.TabIndex = 1;
            // 
            // numYears
            // 
            this.numYears.Location = new System.Drawing.Point(68, 50);
            this.numYears.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numYears.Name = "numYears";
            this.numYears.Size = new System.Drawing.Size(47, 20);
            this.numYears.TabIndex = 3;
            this.numYears.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(121, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "year(s)";
            // 
            // numMonths
            // 
            this.numMonths.Location = new System.Drawing.Point(165, 50);
            this.numMonths.Maximum = new decimal(new int[] {
            11,
            0,
            0,
            0});
            this.numMonths.Name = "numMonths";
            this.numMonths.Size = new System.Drawing.Size(47, 20);
            this.numMonths.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(219, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "month(s)";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(239, 226);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(158, 226);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupPartialPlan
            // 
            this.groupPartialPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPartialPlan.Controls.Add(this.label5);
            this.groupPartialPlan.Controls.Add(this.label3);
            this.groupPartialPlan.Controls.Add(this.comboStartSkill);
            this.groupPartialPlan.Controls.Add(this.label4);
            this.groupPartialPlan.Controls.Add(this.numYears);
            this.groupPartialPlan.Controls.Add(this.numMonths);
            this.groupPartialPlan.Controls.Add(this.label2);
            this.groupPartialPlan.Enabled = false;
            this.groupPartialPlan.Location = new System.Drawing.Point(46, 118);
            this.groupPartialPlan.Name = "groupPartialPlan";
            this.groupPartialPlan.Size = new System.Drawing.Size(268, 82);
            this.groupPartialPlan.TabIndex = 4;
            this.groupPartialPlan.TabStop = false;
            this.groupPartialPlan.Text = "Partial plan settings";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label6.Location = new System.Drawing.Point(12, 273);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(302, 29);
            this.label6.TabIndex = 7;
            this.label6.Text = "(*) Learning skills\'s training time will be taken into account but your character" +
                " will start with his/her current bonuses";
            // 
            // AttributesOptimizationSettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(326, 311);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupPartialPlan);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioPartialPlan);
            this.Controls.Add(this.radioWholePlan);
            this.Controls.Add(this.radioTrainedSKills);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttributesOptimizationSettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Attributes optimization settings";
            ((System.ComponentModel.ISupportInitialize)(this.numYears)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonths)).EndInit();
            this.groupPartialPlan.ResumeLayout(false);
            this.groupPartialPlan.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioTrainedSKills;
        private System.Windows.Forms.RadioButton radioWholePlan;
        private System.Windows.Forms.RadioButton radioPartialPlan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboStartSkill;
        private System.Windows.Forms.NumericUpDown numYears;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMonths;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupPartialPlan;
        private System.Windows.Forms.Label label6;
    }
}
