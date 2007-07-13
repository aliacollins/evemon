namespace EVEMon.SkillPlanner
{
    partial class RequiredSkillsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequiredSkillsControl));
            this.btnAddSkills = new System.Windows.Forms.Button();
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTimeRequired = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tvSkillList = new EVEMon.SkillPlanner.ReqSkillsTreeView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddSkills
            // 
            this.btnAddSkills.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSkills.Location = new System.Drawing.Point(153, 4);
            this.btnAddSkills.Name = "btnAddSkills";
            this.btnAddSkills.Size = new System.Drawing.Size(88, 23);
            this.btnAddSkills.TabIndex = 0;
            this.btnAddSkills.Text = "Add All To Plan";
            this.btnAddSkills.UseVisualStyleBackColor = true;
            this.btnAddSkills.Click += new System.EventHandler(this.btnAddSkills_Click);
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "Cross");
            this.ilIcons.Images.SetKeyName(1, "Tick");
            this.ilIcons.Images.SetKeyName(2, "Planned");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblTimeRequired);
            this.panel1.Controls.Add(this.btnAddSkills);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 127);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(244, 30);
            this.panel1.TabIndex = 4;
            // 
            // lblTimeRequired
            // 
            this.lblTimeRequired.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTimeRequired.AutoSize = true;
            this.lblTimeRequired.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimeRequired.Location = new System.Drawing.Point(3, 9);
            this.lblTimeRequired.Name = "lblTimeRequired";
            this.lblTimeRequired.Size = new System.Drawing.Size(84, 13);
            this.lblTimeRequired.TabIndex = 1;
            this.lblTimeRequired.Text = "Time required";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tvSkillList);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(244, 127);
            this.panel2.TabIndex = 5;
            // 
            // tvSkillList
            // 
            this.tvSkillList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSkillList.FullRowSelect = true;
            this.tvSkillList.ImageIndex = 0;
            this.tvSkillList.ImageList = this.ilIcons;
            this.tvSkillList.Location = new System.Drawing.Point(0, 0);
            this.tvSkillList.Name = "tvSkillList";
            this.tvSkillList.SelectedImageIndex = 0;
            this.tvSkillList.Size = new System.Drawing.Size(244, 127);
            this.tvSkillList.TabIndex = 0;
            this.tvSkillList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvSkillList_NodeMouseDoubleClick);
            // 
            // RequiredSkillsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "RequiredSkillsControl";
            this.Size = new System.Drawing.Size(244, 157);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAddSkills;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTimeRequired;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ImageList ilIcons;
        private ReqSkillsTreeView tvSkillList;
    }
}
