namespace EVEMon
{
    partial class EveMessageWindow
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
            this.readingPane = new EVEMon.ReadingPane();
            this.SuspendLayout();
            // 
            // readingPane
            // 
            this.readingPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.readingPane.Location = new System.Drawing.Point(0, 0);
            this.readingPane.MinimumSize = new System.Drawing.Size(250, 150);
            this.readingPane.Name = "readingPane";
            this.readingPane.Size = new System.Drawing.Size(414, 232);
            this.readingPane.TabIndex = 0;
            // 
            // EveMessageWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 232);
            this.Controls.Add(this.readingPane);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MinimumSize = new System.Drawing.Size(400, 250);
            this.Name = "EveMessageWindow";
            this.Text = "EVE Mail Message";
            this.ResumeLayout(false);

        }

        #endregion

        private ReadingPane readingPane;
    }
}
