namespace EVEMon
{
    partial class EveMailWindow
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
            this.eveMailReadingPane = new EVEMon.EveMailReadingPane();
            this.SuspendLayout();
            // 
            // eveMailReadingPane
            // 
            this.eveMailReadingPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eveMailReadingPane.Location = new System.Drawing.Point(0, 0);
            this.eveMailReadingPane.Name = "eveMailReadingPane";
            this.eveMailReadingPane.Size = new System.Drawing.Size(584, 232);
            this.eveMailReadingPane.TabIndex = 0;
            // 
            // EVEMailWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 232);
            this.Controls.Add(this.eveMailReadingPane);
            this.Name = "EVEMailWindow";
            this.Text = "EVE Mail Message";
            this.ResumeLayout(false);

        }

        #endregion

        private EveMailReadingPane eveMailReadingPane;
    }
}
