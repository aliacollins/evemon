namespace EVEMon.AccountManagement
{
	partial class AccountManagement
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
			this.bLoginSSO = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// bLoginSSO
			// 
			this.bLoginSSO.BackColor = System.Drawing.Color.Black;
			this.bLoginSSO.ForeColor = System.Drawing.Color.White;
			this.bLoginSSO.Location = new System.Drawing.Point(314, 218);
			this.bLoginSSO.Name = "bLoginSSO";
			this.bLoginSSO.Size = new System.Drawing.Size(451, 128);
			this.bLoginSSO.TabIndex = 0;
			this.bLoginSSO.Text = "Log in to EVE Online";
			this.bLoginSSO.UseVisualStyleBackColor = false;
			this.bLoginSSO.Click += new System.EventHandler(this.bLoginSSO_Click);
			// 
			// AccountManagement
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1424, 734);
			this.Controls.Add(this.bLoginSSO);
			this.Name = "AccountManagement";
			this.Text = "AccountManagement";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bLoginSSO;
	}
}