namespace EVEMon.SkillPlanner
{
    partial class PlanSelectWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlanSelectWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ofdOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbPlanList = new EVEMon.SkillPlanner.DraggableListView();
            this.PlanName = new System.Windows.Forms.ColumnHeader();
            this.PlanDate = new System.Windows.Forms.ColumnHeader();
            this.PlanSkills = new System.Windows.Forms.ColumnHeader();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiRename = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsbMoveUp = new System.Windows.Forms.ToolStripButton();
            this.tsbMoveDown = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOpen = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miNewPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.miLoadPlanFromFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miLoadPlanFromCharacter = new System.Windows.Forms.ToolStripMenuItem();
            this.mEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.miRename = new System.Windows.Forms.ToolStripMenuItem();
            this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select a plan to open, or multiple plans to merge:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(84, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ofdOpenDialog
            // 
            this.ofdOpenDialog.Filter = "Plan Files (*.emp, *.xml)|*.xml;*.emp|All Files (*.*)|*.*";
            this.ofdOpenDialog.Title = "Open Plan File";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.lbPlanList);
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Location = new System.Drawing.Point(9, 22);
            this.panel1.MinimumSize = new System.Drawing.Size(305, 155);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(473, 327);
            this.panel1.TabIndex = 6;
            // 
            // lbPlanList
            // 
            this.lbPlanList.AllowDrop = true;
            this.lbPlanList.AllowRowReorder = true;
            this.lbPlanList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PlanName,
            this.PlanDate,
            this.PlanSkills});
            this.lbPlanList.ContextMenuStrip = this.contextMenu;
            this.lbPlanList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPlanList.FullRowSelect = true;
            this.lbPlanList.Location = new System.Drawing.Point(0, 0);
            this.lbPlanList.Name = "lbPlanList";
            this.lbPlanList.Size = new System.Drawing.Size(435, 327);
            this.lbPlanList.TabIndex = 2;
            this.lbPlanList.UseCompatibleStateImageBehavior = false;
            this.lbPlanList.View = System.Windows.Forms.View.Details;
            this.lbPlanList.DoubleClick += new System.EventHandler(this.lbPlanList_DoubleClick);
            this.lbPlanList.SelectedIndexChanged += new System.EventHandler(this.lbPlanList_SelectedIndexChanged);
            this.lbPlanList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lbPlanList_ColumnClick);
            this.lbPlanList.ListViewItemsDragging += new System.EventHandler<EVEMon.SkillPlanner.ListViewDragEventArgs>(this.lbPlanList_ListViewItemsDragging);
            // 
            // PlanName
            // 
            this.PlanName.Text = "Plan Name";
            this.PlanName.Width = 176;
            // 
            // PlanDate
            // 
            this.PlanDate.Text = "Completion Time";
            this.PlanDate.Width = 197;
            // 
            // PlanSkills
            // 
            this.PlanSkills.Text = "Skills";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiOpen,
            this.cmiRename,
            this.cmiDelete});
            this.contextMenu.Name = "contextMenuStrip1";
            this.contextMenu.Size = new System.Drawing.Size(125, 70);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // cmiOpen
            // 
            this.cmiOpen.Name = "cmiOpen";
            this.cmiOpen.Size = new System.Drawing.Size(124, 22);
            this.cmiOpen.Text = "Open";
            this.cmiOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // cmiRename
            // 
            this.cmiRename.Name = "cmiRename";
            this.cmiRename.Size = new System.Drawing.Size(124, 22);
            this.cmiRename.Text = "Rename";
            this.cmiRename.Click += new System.EventHandler(this.miRename_Click);
            // 
            // cmiDelete
            // 
            this.cmiDelete.Name = "cmiDelete";
            this.cmiDelete.Size = new System.Drawing.Size(124, 22);
            this.cmiDelete.Text = "Delete";
            this.cmiDelete.Click += new System.EventHandler(this.miDelete_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tsbMoveUp,
            this.tsbMoveDown});
            this.toolStrip2.Location = new System.Drawing.Point(435, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(38, 327);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(35, 13);
            this.toolStripLabel1.Text = "Move:";
            // 
            // tsbMoveUp
            // 
            this.tsbMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMoveUp.Enabled = false;
            this.tsbMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("tsbMoveUp.Image")));
            this.tsbMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMoveUp.Name = "tsbMoveUp";
            this.tsbMoveUp.Size = new System.Drawing.Size(35, 20);
            this.tsbMoveUp.Text = "Move Up";
            this.tsbMoveUp.Click += new System.EventHandler(this.tsbMoveUp_Click);
            // 
            // tsbMoveDown
            // 
            this.tsbMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMoveDown.Enabled = false;
            this.tsbMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("tsbMoveDown.Image")));
            this.tsbMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMoveDown.Name = "tsbMoveDown";
            this.tsbMoveDown.Size = new System.Drawing.Size(35, 20);
            this.tsbMoveDown.Text = "Move Down";
            this.tsbMoveDown.Click += new System.EventHandler(this.tsbMoveDown_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(6, 6, 0, 12);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(484, 401);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnOpen);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(317, 360);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 8, 6, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(162, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOpen.Enabled = false;
            this.flowLayoutPanel1.SetFlowBreak(this.btnOpen, true);
            this.btnOpen.Location = new System.Drawing.Point(3, 3);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 4;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFile,
            this.mEdit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(484, 24);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuBar";
            // 
            // mFile
            // 
            this.mFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNewPlan,
            this.miLoadPlanFromFile,
            this.miLoadPlanFromCharacter});
            this.mFile.Name = "mFile";
            this.mFile.Size = new System.Drawing.Size(35, 20);
            this.mFile.Text = "&File";
            this.mFile.DropDownOpening += new System.EventHandler(this.mFile_DropDownOpening);
            // 
            // miNewPlan
            // 
            this.miNewPlan.Name = "miNewPlan";
            this.miNewPlan.Size = new System.Drawing.Size(216, 22);
            this.miNewPlan.Text = "&New Plan…";
            this.miNewPlan.Click += new System.EventHandler(this.miNewPlan_Click);
            // 
            // miLoadPlanFromFile
            // 
            this.miLoadPlanFromFile.Image = ((System.Drawing.Image)(resources.GetObject("miLoadPlanFromFile.Image")));
            this.miLoadPlanFromFile.Name = "miLoadPlanFromFile";
            this.miLoadPlanFromFile.Size = new System.Drawing.Size(216, 22);
            this.miLoadPlanFromFile.Text = "&Load Plan from File…";
            this.miLoadPlanFromFile.Click += new System.EventHandler(this.miLoadPlanFromFile_Click);
            // 
            // miLoadPlanFromCharacter
            // 
            this.miLoadPlanFromCharacter.Name = "miLoadPlanFromCharacter";
            this.miLoadPlanFromCharacter.Size = new System.Drawing.Size(216, 22);
            this.miLoadPlanFromCharacter.Text = "Load Plan from &Character…";
            this.miLoadPlanFromCharacter.Click += new System.EventHandler(this.miLoadPlanFromCharacter_Click);
            // 
            // mEdit
            // 
            this.mEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRename,
            this.miDelete});
            this.mEdit.Name = "mEdit";
            this.mEdit.Size = new System.Drawing.Size(37, 20);
            this.mEdit.Text = "&Edit";
            this.mEdit.DropDownOpening += new System.EventHandler(this.mEdit_DropDownOpening);
            // 
            // miRename
            // 
            this.miRename.Image = ((System.Drawing.Image)(resources.GetObject("miRename.Image")));
            this.miRename.Name = "miRename";
            this.miRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.miRename.Size = new System.Drawing.Size(152, 22);
            this.miRename.Text = "&Rename";
            this.miRename.Click += new System.EventHandler(this.miRename_Click);
            // 
            // miDelete
            // 
            this.miDelete.Image = ((System.Drawing.Image)(resources.GetObject("miDelete.Image")));
            this.miDelete.Name = "miDelete";
            this.miDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.miDelete.Size = new System.Drawing.Size(152, 22);
            this.miDelete.Text = "&Delete";
            this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
            // 
            // PlanSelectWindow
            // 
            this.AcceptButton = this.btnOpen;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 425);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(364, 331);
            this.Name = "PlanSelectWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Plan";
            this.Load += new System.EventHandler(this.PlanSelectWindow_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenu.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.OpenFileDialog ofdOpenDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton tsbMoveUp;
        private System.Windows.Forms.ToolStripButton tsbMoveDown;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem cmiDelete;
        private System.Windows.Forms.ToolStripMenuItem cmiRename;
        private System.Windows.Forms.ToolStripMenuItem cmiOpen;
        private DraggableListView lbPlanList;
        private System.Windows.Forms.ColumnHeader PlanName;
        private System.Windows.Forms.ColumnHeader PlanDate;
        private System.Windows.Forms.ColumnHeader PlanSkills;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mFile;
        private System.Windows.Forms.ToolStripMenuItem mEdit;
        private System.Windows.Forms.ToolStripMenuItem miLoadPlanFromFile;
        private System.Windows.Forms.ToolStripMenuItem miLoadPlanFromCharacter;
        private System.Windows.Forms.ToolStripMenuItem miRename;
        private System.Windows.Forms.ToolStripMenuItem miDelete;
        private System.Windows.Forms.ToolStripMenuItem miNewPlan;
    }
}
