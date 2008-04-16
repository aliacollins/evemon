namespace EVEMon.SkillPlanner
{
    partial class ItemFittingFilterSummary
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
            this.lblSlot = new System.Windows.Forms.Label();
            this.lblSlotValue = new System.Windows.Forms.Label();
            this.lblCPU = new System.Windows.Forms.Label();
            this.lblCpuValue = new System.Windows.Forms.Label();
            this.lblGrid = new System.Windows.Forms.Label();
            this.lblGridValue = new System.Windows.Forms.Label();
            this.btnConfigure = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblSlot
            // 
            this.lblSlot.AutoSize = true;
            this.lblSlot.Location = new System.Drawing.Point(0, 0);
            this.lblSlot.Name = "lblSlot";
            this.lblSlot.Size = new System.Drawing.Size(28, 13);
            this.lblSlot.TabIndex = 0;
            this.lblSlot.Text = "Slot:";
            // 
            // lblSlotValue
            // 
            this.lblSlotValue.AutoSize = true;
            this.lblSlotValue.Location = new System.Drawing.Point(30, 0);
            this.lblSlotValue.Name = "lblSlotValue";
            this.lblSlotValue.Size = new System.Drawing.Size(24, 13);
            this.lblSlotValue.TabIndex = 1;
            this.lblSlotValue.Text = "any";
            // 
            // lblCPU
            // 
            this.lblCPU.AutoSize = true;
            this.lblCPU.Location = new System.Drawing.Point(0, 16);
            this.lblCPU.Name = "lblCPU";
            this.lblCPU.Size = new System.Drawing.Size(32, 13);
            this.lblCPU.TabIndex = 2;
            this.lblCPU.Text = "CPU:";
            // 
            // lblCpuValue
            // 
            this.lblCpuValue.AutoSize = true;
            this.lblCpuValue.Location = new System.Drawing.Point(30, 16);
            this.lblCpuValue.Name = "lblCpuValue";
            this.lblCpuValue.Size = new System.Drawing.Size(39, 13);
            this.lblCpuValue.TabIndex = 3;
            this.lblCpuValue.Text = "no limit";
            // 
            // lblGrid
            // 
            this.lblGrid.AutoSize = true;
            this.lblGrid.Location = new System.Drawing.Point(0, 32);
            this.lblGrid.Name = "lblGrid";
            this.lblGrid.Size = new System.Drawing.Size(29, 13);
            this.lblGrid.TabIndex = 4;
            this.lblGrid.Text = "Grid:";
            // 
            // lblGridValue
            // 
            this.lblGridValue.AutoSize = true;
            this.lblGridValue.Location = new System.Drawing.Point(30, 32);
            this.lblGridValue.Name = "lblGridValue";
            this.lblGridValue.Size = new System.Drawing.Size(39, 13);
            this.lblGridValue.TabIndex = 5;
            this.lblGridValue.Text = "no limit";
            // 
            // btnConfigure
            // 
            this.btnConfigure.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnConfigure.Location = new System.Drawing.Point(93, 11);
            this.btnConfigure.Name = "btnConfigure";
            this.btnConfigure.Size = new System.Drawing.Size(66, 23);
            this.btnConfigure.TabIndex = 6;
            this.btnConfigure.Text = "Configure";
            this.btnConfigure.UseVisualStyleBackColor = true;
            this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
            // 
            // ItemFittingFilterSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnConfigure);
            this.Controls.Add(this.lblSlot);
            this.Controls.Add(this.lblSlotValue);
            this.Controls.Add(this.lblCPU);
            this.Controls.Add(this.lblCpuValue);
            this.Controls.Add(this.lblGrid);
            this.Controls.Add(this.lblGridValue);
            this.Name = "ItemFittingFilterSummary";
            this.Size = new System.Drawing.Size(162, 47);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSlot;
        private System.Windows.Forms.Label lblSlotValue;
        private System.Windows.Forms.Label lblCPU;
        private System.Windows.Forms.Label lblCpuValue;
        private System.Windows.Forms.Label lblGrid;
        private System.Windows.Forms.Label lblGridValue;
        private System.Windows.Forms.Button btnConfigure;

    }
}
