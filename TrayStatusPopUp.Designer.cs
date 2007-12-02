namespace EVEMon
{
    partial class TrayStatusPopUp
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
            this.components = new System.ComponentModel.Container();
            this.displayTimer = new System.Windows.Forms.Timer(this.components);
            this.lblTQStatus = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlCharInfo = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayTimer
            // 
            this.displayTimer.Tick += new System.EventHandler(this.displayTimer_Tick);
            // 
            // lblTQStatus
            // 
            this.lblTQStatus.AutoSize = true;
            this.lblTQStatus.Location = new System.Drawing.Point(3, 16);
            this.lblTQStatus.Margin = new System.Windows.Forms.Padding(3, 10, 3, 5);
            this.lblTQStatus.Name = "lblTQStatus";
            this.lblTQStatus.Size = new System.Drawing.Size(100, 13);
            this.lblTQStatus.TabIndex = 1;
            this.lblTQStatus.Text = "TQ Status Message";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel1.Controls.Add(this.pnlCharInfo);
            this.flowLayoutPanel1.Controls.Add(this.lblTQStatus);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(106, 34);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // pnlCharInfo
            // 
            this.pnlCharInfo.AutoSize = true;
            this.pnlCharInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlCharInfo.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlCharInfo.Location = new System.Drawing.Point(3, 3);
            this.pnlCharInfo.Name = "pnlCharInfo";
            this.pnlCharInfo.Size = new System.Drawing.Size(0, 0);
            this.pnlCharInfo.TabIndex = 3;
            // 
            // TrayStatusPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.ControlBox = false;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrayStatusPopUp";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "EveMon Status";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.SystemColors.Info;
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer displayTimer;
        private System.Windows.Forms.Label lblTQStatus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel pnlCharInfo;
    }
}
