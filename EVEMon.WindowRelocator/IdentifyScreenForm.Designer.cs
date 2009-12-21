namespace EVEMon.WindowRelocator
{
    partial class IdentifyScreenForm
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
            this.pbScreenNumber = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreenNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // pbScreenNumber
            // 
            this.pbScreenNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbScreenNumber.Location = new System.Drawing.Point(0, 0);
            this.pbScreenNumber.Margin = new System.Windows.Forms.Padding(0);
            this.pbScreenNumber.Name = "pbScreenNumber";
            this.pbScreenNumber.Size = new System.Drawing.Size(512, 512);
            this.pbScreenNumber.TabIndex = 0;
            this.pbScreenNumber.TabStop = false;
            // 
            // IdentifyScreenForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Magenta;
            this.ClientSize = new System.Drawing.Size(512, 512);
            this.Controls.Add(this.pbScreenNumber);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "IdentifyScreenForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TransparencyKey = System.Drawing.Color.Magenta;
            this.Load += new System.EventHandler(this.IdentifyScreenForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreenNumber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbScreenNumber;
    }
}