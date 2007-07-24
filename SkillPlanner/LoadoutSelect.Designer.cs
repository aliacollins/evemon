
namespace EVEMon.SkillPlanner
{
    partial class LoadoutSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadoutSelect));
            this.lvLoadouts = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colAuthor = new System.Windows.Forms.ColumnHeader();
            this.colRating = new System.Windows.Forms.ColumnHeader();
            this.colDate = new System.Windows.Forms.ColumnHeader();
            this.lblShip = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvLoadouts
            // 
            this.lvLoadouts.AllowColumnReorder = true;
            this.lvLoadouts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colAuthor,
            this.colRating,
            this.colDate});
            this.lvLoadouts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLoadouts.FullRowSelect = true;
            this.lvLoadouts.Location = new System.Drawing.Point(0, 0);
            this.lvLoadouts.MinimumSize = new System.Drawing.Size(465, 190);
            this.lvLoadouts.Name = "lvLoadouts";
            this.lvLoadouts.Size = new System.Drawing.Size(470, 190);
            this.lvLoadouts.TabIndex = 0;
            this.lvLoadouts.UseCompatibleStateImageBehavior = false;
            this.lvLoadouts.View = System.Windows.Forms.View.Details;
            this.lvLoadouts.DoubleClick += new System.EventHandler(this.lvLoadouts_DoubleClick);
            this.lvLoadouts.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvLoadouts_ColumnClick);
            this.lvLoadouts.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvLoadouts_ItemSelectionChanged);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 205;
            // 
            // colAuthor
            // 
            this.colAuthor.Text = "Author";
            this.colAuthor.Width = 113;
            // 
            // colRating
            // 
            this.colRating.Text = "Rating";
            this.colRating.Width = 56;
            // 
            // colDate
            // 
            this.colDate.Text = "Date";
            this.colDate.Width = 90;
            // 
            // lblShip
            // 
            this.lblShip.AutoSize = true;
            this.lblShip.Location = new System.Drawing.Point(12, 9);
            this.lblShip.Name = "lblShip";
            this.lblShip.Size = new System.Drawing.Size(100, 13);
            this.lblShip.TabIndex = 1;
            this.lblShip.Text = "{0} Loadouts for {1}";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(385, 231);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(304, 231);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 4;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.lvLoadouts);
            this.panel2.Location = new System.Drawing.Point(1, 31);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(470, 185);
            this.panel2.TabIndex = 5;
            // 
            // LoadoutSelect
            // 
            this.ClientSize = new System.Drawing.Size(472, 266);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblShip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(480, 300);
            this.Name = "LoadoutSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Battleclinic Loadout Selection";
            this.Load += new System.EventHandler(this.LoadoutSelect_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvLoadouts;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colRating;
        private System.Windows.Forms.Label lblShip;
        private System.Windows.Forms.ColumnHeader colAuthor;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.ColumnHeader colDate;
        private System.Windows.Forms.Panel panel2;
    }
}