
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadoutSelect));
            this.lblShip = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.persistentSplitContainer1 = new EVEMon.SkillPlanner.PersistentSplitContainer();
            this.lvLoadouts = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colAuthor = new System.Windows.Forms.ColumnHeader();
            this.colRating = new System.Windows.Forms.ColumnHeader();
            this.colDate = new System.Windows.Forms.ColumnHeader();
            this.tvLoadout = new System.Windows.Forms.TreeView();
            this.lbDate = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblForum = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblShipType = new System.Windows.Forms.Label();
            this.pbShip = new System.Windows.Forms.PictureBox();
            this.lblPlanned = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTrainTime = new System.Windows.Forms.Label();
            this.btnPlan = new System.Windows.Forms.Button();
            this.cmNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miShowInBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLoad = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.persistentSplitContainer1.Panel1.SuspendLayout();
            this.persistentSplitContainer1.Panel2.SuspendLayout();
            this.persistentSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbShip)).BeginInit();
            this.cmNode.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblShip
            // 
            this.lblShip.AutoSize = true;
            this.lblShip.Location = new System.Drawing.Point(109, 82);
            this.lblShip.Name = "lblShip";
            this.lblShip.Size = new System.Drawing.Size(68, 13);
            this.lblShip.TabIndex = 1;
            this.lblShip.Text = "{0} Loadouts";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(641, 471);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.persistentSplitContainer1);
            this.panel1.Location = new System.Drawing.Point(0, 123);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(716, 342);
            this.panel1.TabIndex = 6;
            // 
            // persistentSplitContainer1
            // 
            this.persistentSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.persistentSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.persistentSplitContainer1.Name = "persistentSplitContainer1";
            // 
            // persistentSplitContainer1.Panel1
            // 
            this.persistentSplitContainer1.Panel1.Controls.Add(this.lvLoadouts);
            this.persistentSplitContainer1.Panel1MinSize = 350;
            // 
            // persistentSplitContainer1.Panel2
            // 
            this.persistentSplitContainer1.Panel2.Controls.Add(this.tvLoadout);
            this.persistentSplitContainer1.RememberDistanceKey = null;
            this.persistentSplitContainer1.Size = new System.Drawing.Size(716, 342);
            this.persistentSplitContainer1.SplitterDistance = 373;
            this.persistentSplitContainer1.TabIndex = 5;
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
            this.lvLoadouts.MinimumSize = new System.Drawing.Size(300, 190);
            this.lvLoadouts.Name = "lvLoadouts";
            this.lvLoadouts.Size = new System.Drawing.Size(373, 342);
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
            this.colName.Width = 153;
            // 
            // colAuthor
            // 
            this.colAuthor.Text = "Author";
            this.colAuthor.Width = 68;
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
            // tvLoadout
            // 
            this.tvLoadout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvLoadout.Location = new System.Drawing.Point(0, 0);
            this.tvLoadout.Name = "tvLoadout";
            this.tvLoadout.Size = new System.Drawing.Size(339, 342);
            this.tvLoadout.TabIndex = 3;
            this.tvLoadout.DoubleClick += new System.EventHandler(this.tvLoadout_DoubleClick);
            this.tvLoadout.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvLoadout_MouseUp);
            // 
            // lbDate
            // 
            this.lbDate.AutoSize = true;
            this.lbDate.Location = new System.Drawing.Point(199, 51);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(35, 13);
            this.lbDate.TabIndex = 25;
            this.lbDate.Text = "label6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(109, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Submission Date:";
            // 
            // lblForum
            // 
            this.lblForum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblForum.AutoSize = true;
            this.lblForum.Location = new System.Drawing.Point(615, 96);
            this.lblForum.Name = "lblForum";
            this.lblForum.Size = new System.Drawing.Size(101, 13);
            this.lblForum.TabIndex = 23;
            this.lblForum.TabStop = true;
            this.lblForum.Text = "Discuss this loadout";
            this.lblForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblForum_LinkClicked);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(109, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Author:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(109, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Loadout Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(109, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Ship:";
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(199, 38);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(35, 13);
            this.lblAuthor.TabIndex = 19;
            this.lblAuthor.Text = "label1";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(199, 25);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 18;
            this.lblName.Text = "label2";
            // 
            // lblShipType
            // 
            this.lblShipType.AutoSize = true;
            this.lblShipType.Location = new System.Drawing.Point(199, 12);
            this.lblShipType.Name = "lblShipType";
            this.lblShipType.Size = new System.Drawing.Size(35, 13);
            this.lblShipType.TabIndex = 17;
            this.lblShipType.Text = "label1";
            // 
            // pbShip
            // 
            this.pbShip.Location = new System.Drawing.Point(12, 12);
            this.pbShip.Name = "pbShip";
            this.pbShip.Size = new System.Drawing.Size(91, 83);
            this.pbShip.TabIndex = 16;
            this.pbShip.TabStop = false;
            // 
            // lblPlanned
            // 
            this.lblPlanned.AutoSize = true;
            this.lblPlanned.Location = new System.Drawing.Point(307, 481);
            this.lblPlanned.Name = "lblPlanned";
            this.lblPlanned.Size = new System.Drawing.Size(0, 13);
            this.lblPlanned.TabIndex = 28;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(128, 481);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(173, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "Training Time for selected loadout: ";
            // 
            // lblTrainTime
            // 
            this.lblTrainTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTrainTime.AutoSize = true;
            this.lblTrainTime.Location = new System.Drawing.Point(307, 481);
            this.lblTrainTime.Name = "lblTrainTime";
            this.lblTrainTime.Size = new System.Drawing.Size(35, 13);
            this.lblTrainTime.TabIndex = 27;
            this.lblTrainTime.Text = "label2";
            // 
            // btnPlan
            // 
            this.btnPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlan.Enabled = false;
            this.btnPlan.Location = new System.Drawing.Point(560, 471);
            this.btnPlan.Name = "btnPlan";
            this.btnPlan.Size = new System.Drawing.Size(75, 23);
            this.btnPlan.TabIndex = 29;
            this.btnPlan.Text = "Add To Plan";
            this.btnPlan.UseVisualStyleBackColor = true;
            this.btnPlan.Click += new System.EventHandler(this.btnPlan_Click);
            // 
            // cmNode
            // 
            this.cmNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miShowInBrowser});
            this.cmNode.Name = "cmNode";
            this.cmNode.Size = new System.Drawing.Size(204, 26);
            // 
            // miShowInBrowser
            // 
            this.miShowInBrowser.Name = "miShowInBrowser";
            this.miShowInBrowser.Size = new System.Drawing.Size(203, 22);
            this.miShowInBrowser.Text = "Show Item In Browser...";
            this.miShowInBrowser.Click += new System.EventHandler(this.tvLoadout_DoubleClick);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoad.Location = new System.Drawing.Point(12, 476);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(110, 23);
            this.btnLoad.TabIndex = 30;
            this.btnLoad.Text = "View Loadout";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // LoadoutSelect
            // 
            this.ClientSize = new System.Drawing.Size(728, 506);
            this.Controls.Add(this.btnPlan);
            this.Controls.Add(this.lblPlanned);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblTrainTime);
            this.Controls.Add(this.lblForum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblShipType);
            this.Controls.Add(this.pbShip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblShip);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnLoad);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(480, 300);
            this.Name = "LoadoutSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Battleclinic Loadout Selection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoadoutSelect_FormClosing);
            this.Load += new System.EventHandler(this.LoadoutSelect_Load);
            this.panel1.ResumeLayout(false);
            this.persistentSplitContainer1.Panel1.ResumeLayout(false);
            this.persistentSplitContainer1.Panel2.ResumeLayout(false);
            this.persistentSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbShip)).EndInit();
            this.cmNode.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblShip;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListView lvLoadouts;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colAuthor;
        private System.Windows.Forms.ColumnHeader colRating;
        private System.Windows.Forms.ColumnHeader colDate;
        private PersistentSplitContainer persistentSplitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel lblForum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblShipType;
        private System.Windows.Forms.PictureBox pbShip;
        private System.Windows.Forms.TreeView tvLoadout;
        private System.Windows.Forms.Label lblPlanned;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTrainTime;
        private System.Windows.Forms.Button btnPlan;
        private System.Windows.Forms.ContextMenuStrip cmNode;
        private System.Windows.Forms.ToolStripMenuItem miShowInBrowser;
        private System.Windows.Forms.Button btnLoad;
    }
}