namespace EVEMon.Common
{
    partial class EveImage
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
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pbImage
            // 
            this.pbImage.Cursor = System.Windows.Forms.Cursors.Default;
            this.pbImage.Location = new System.Drawing.Point(0, 0);
            this.pbImage.Margin = new System.Windows.Forms.Padding(0);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(100, 50);
            this.pbImage.TabIndex = 0;
            this.pbImage.TabStop = false;
            this.toolTip1.SetToolTip(this.pbImage, "Double-click to view a larger image");
            this.pbImage.DoubleClick += new System.EventHandler(this.pbImage_DoubleClick);
            // 
            // EveImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbImage);
            this.Name = "EveImage";
            this.Size = new System.Drawing.Size(287, 284);
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
