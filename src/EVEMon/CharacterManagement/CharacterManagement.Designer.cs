namespace EVEMon.CharacterManagement
{
	partial class CharacterManagement
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
			this.listView1 = new System.Windows.Forms.ListView();
			this.CharacterName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.LastRefreshed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// bLoginSSO
			// 
			this.bLoginSSO.BackColor = System.Drawing.Color.Black;
			this.bLoginSSO.ForeColor = System.Drawing.Color.White;
			this.bLoginSSO.Location = new System.Drawing.Point(24, 44);
			this.bLoginSSO.Name = "bLoginSSO";
			this.bLoginSSO.Size = new System.Drawing.Size(305, 113);
			this.bLoginSSO.TabIndex = 0;
			this.bLoginSSO.Text = "Add a Charater";
			this.bLoginSSO.UseVisualStyleBackColor = false;
			this.bLoginSSO.Click += new System.EventHandler(this.bLoginSSO_Click);
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CharacterName,
            this.LastRefreshed});
			this.listView1.Location = new System.Drawing.Point(396, 44);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(451, 574);
			this.listView1.TabIndex = 1;
			this.listView1.UseCompatibleStateImageBehavior = false;
			// 
			// CharacterName
			// 
			this.CharacterName.Text = "Character Name";
			// 
			// LastRefreshed
			// 
			this.LastRefreshed.Text = "Last Refreshed";
			// 
			// CharacterManagement
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1424, 734);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.bLoginSSO);
			this.Name = "CharacterManagement";
			this.Text = "Character Management";
			this.Load += new System.EventHandler(this.CharacterManagement_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bLoginSSO;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader CharacterName;
		private System.Windows.Forms.ColumnHeader LastRefreshed;
	}
}