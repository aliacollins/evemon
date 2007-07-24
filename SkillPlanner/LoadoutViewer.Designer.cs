namespace EVEMon.SkillPlanner
{
    partial class LoadoutViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadoutViewer));
            this.lblShip = new System.Windows.Forms.Label();
            this.tvLoadout = new System.Windows.Forms.TreeView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPlan = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.pbShip = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbTrainTime = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblForum = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.lblPlanned = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbShip)).BeginInit();
            this.SuspendLayout();
            // 
            // lblShip
            // 
            this.lblShip.AutoSize = true;
            this.lblShip.Location = new System.Drawing.Point(198, 13);
            this.lblShip.Name = "lblShip";
            this.lblShip.Size = new System.Drawing.Size(35, 13);
            this.lblShip.TabIndex = 1;
            this.lblShip.Text = "label1";
            // 
            // tvLoadout
            // 
            this.tvLoadout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvLoadout.Location = new System.Drawing.Point(11, 112);
            this.tvLoadout.Name = "tvLoadout";
            this.tvLoadout.Size = new System.Drawing.Size(324, 401);
            this.tvLoadout.TabIndex = 2;
            this.tvLoadout.DoubleClick += new System.EventHandler(this.tvLoadout_DoubleClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(260, 557);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPlan
            // 
            this.btnPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlan.Enabled = false;
            this.btnPlan.Location = new System.Drawing.Point(179, 557);
            this.btnPlan.Name = "btnPlan";
            this.btnPlan.Size = new System.Drawing.Size(75, 23);
            this.btnPlan.TabIndex = 4;
            this.btnPlan.Text = "Add To Plan";
            this.btnPlan.UseVisualStyleBackColor = true;
            this.btnPlan.Click += new System.EventHandler(this.btnPlan_Click);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(198, 26);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "label2";
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Location = new System.Drawing.Point(198, 39);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(35, 13);
            this.lblAuthor.TabIndex = 7;
            this.lblAuthor.Text = "label1";
            // 
            // pbShip
            // 
            this.pbShip.Location = new System.Drawing.Point(11, 13);
            this.pbShip.Name = "pbShip";
            this.pbShip.Size = new System.Drawing.Size(91, 83);
            this.pbShip.TabIndex = 0;
            this.pbShip.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 524);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Training Time for this loadout: ";
            // 
            // lbTrainTime
            // 
            this.lbTrainTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbTrainTime.AutoSize = true;
            this.lbTrainTime.Location = new System.Drawing.Point(162, 524);
            this.lbTrainTime.Name = "lbTrainTime";
            this.lbTrainTime.Size = new System.Drawing.Size(35, 13);
            this.lbTrainTime.TabIndex = 9;
            this.lbTrainTime.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(108, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Ship:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(108, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Loadout Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(108, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Author:";
            // 
            // lblForum
            // 
            this.lblForum.AutoSize = true;
            this.lblForum.Location = new System.Drawing.Point(108, 83);
            this.lblForum.Name = "lblForum";
            this.lblForum.Size = new System.Drawing.Size(101, 13);
            this.lblForum.TabIndex = 13;
            this.lblForum.TabStop = true;
            this.lblForum.Text = "Discuss this loadout";
            this.lblForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblForum_LinkClicked);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(108, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Submission Date:";
            // 
            // lbDate
            // 
            this.lbDate.AutoSize = true;
            this.lbDate.Location = new System.Drawing.Point(198, 52);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(35, 13);
            this.lbDate.TabIndex = 15;
            this.lbDate.Text = "label6";
            // 
            // lblPlanned
            // 
            this.lblPlanned.AutoSize = true;
            this.lblPlanned.Location = new System.Drawing.Point(12, 537);
            this.lblPlanned.Name = "lblPlanned";
            this.lblPlanned.Size = new System.Drawing.Size(10, 13);
            this.lblPlanned.TabIndex = 16;
            this.lblPlanned.Text = " ";
            // 
            // LoadoutViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(364, 592);
            this.Controls.Add(this.lblPlanned);
            this.Controls.Add(this.lbDate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblForum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbTrainTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.btnPlan);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tvLoadout);
            this.Controls.Add(this.lblShip);
            this.Controls.Add(this.pbShip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(305, 514);
            this.Name = "LoadoutViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoadoutViewer";
            this.Load += new System.EventHandler(this.LoadoutViewer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbShip)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbShip;
        private System.Windows.Forms.Label lblShip;
        private System.Windows.Forms.TreeView tvLoadout;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnPlan;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbTrainTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel lblForum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lblPlanned;
    }
}