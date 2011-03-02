namespace EVEMon.BlankCharacter
{
    partial class BlankCharacterWindow
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.blankCharacterControl = new EVEMon.BlankCharacter.BlankCharacterControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(402, 23);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 20, 20, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(321, 23);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "Add";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 425);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(497, 57);
            this.panel1.TabIndex = 2;
            // 
            // blankCharacterControl
            // 
            this.blankCharacterControl.BackColor = System.Drawing.Color.Transparent;
            this.blankCharacterControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blankCharacterControl.ForeColor = System.Drawing.SystemColors.GrayText;
            this.blankCharacterControl.Location = new System.Drawing.Point(0, 0);
            this.blankCharacterControl.Name = "blankCharacterControl";
            this.blankCharacterControl.Size = new System.Drawing.Size(497, 425);
            this.blankCharacterControl.TabIndex = 3;
            // 
            // BlankCharacterWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(497, 482);
            this.Controls.Add(this.blankCharacterControl);
            this.Controls.Add(this.panel1);
            this.Name = "BlankCharacterWindow";
            this.Text = "Blank Character Creator";
            this.Load += new System.EventHandler(this.BlankCharacterWindow_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private BlankCharacterControl blankCharacterControl;
        private System.Windows.Forms.Panel panel1;
    }
}