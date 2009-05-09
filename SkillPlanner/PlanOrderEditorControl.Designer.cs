namespace EVEMon.SkillPlanner
{
    partial class PlanOrderEditorControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlanOrderEditorControl));
            this.cmsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miChangeNote = new System.Windows.Forms.ToolStripMenuItem();
            this.miChangePriority = new System.Windows.Forms.ToolStripMenuItem();
            this.miShowInSkillBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.miShowInSkillExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.miMarkOwned = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.miSubPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miChangeLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.miChangeTo0 = new System.Windows.Forms.ToolStripMenuItem();
            this.miChangeTo1 = new System.Windows.Forms.ToolStripMenuItem();
            this.miChangeTo2 = new System.Windows.Forms.ToolStripMenuItem();
            this.miChangeTo3 = new System.Windows.Forms.ToolStripMenuItem();
            this.miChangeTo4 = new System.Windows.Forms.ToolStripMenuItem();
            this.miChangeTo5 = new System.Windows.Forms.ToolStripMenuItem();
            this.miRemoveFromPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miPlanGroups = new System.Windows.Forms.ToolStripMenuItem();
            this.sfdSave = new System.Windows.Forms.SaveFileDialog();
            this.tmrAutoRefresh = new System.Windows.Forms.Timer(this.components);
            this.tsPlan = new System.Windows.Forms.ToolStrip();
            this.tslMove = new System.Windows.Forms.ToolStripLabel();
            this.tsbMoveUp = new System.Windows.Forms.ToolStripButton();
            this.tsbMoveDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tslSort = new System.Windows.Forms.ToolStripLabel();
            this.tsSortLearning = new System.Windows.Forms.ToolStripButton();
            this.tsSortPriorities = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbToggleSkills = new System.Windows.Forms.ToolStripButton();
            this.tsbToggleRemapping = new System.Windows.Forms.ToolStripButton();
            this.ilSkillDependency = new System.Windows.Forms.ImageList(this.components);
            this.tmrSelect = new System.Windows.Forms.Timer(this.components);
            this.ilColumnsStates = new System.Windows.Forms.ImageList(this.components);
            this.pscPlan = new EVEMon.SkillPlanner.PersistentSplitContainer();
            this.lvSkills = new EVEMon.SkillPlanner.DraggableListView();
            this.pHeader = new System.Windows.Forms.Panel();
            this.llSuggestionLink = new System.Windows.Forms.LinkLabel();
            this.skillSelectControl = new EVEMon.SkillPlanner.SkillSelectControl();
            this.cmsContextMenu.SuspendLayout();
            this.tsPlan.SuspendLayout();
            this.pscPlan.Panel1.SuspendLayout();
            this.pscPlan.Panel2.SuspendLayout();
            this.pscPlan.SuspendLayout();
            this.pHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsContextMenu
            // 
            this.cmsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miChangeNote,
            this.miChangePriority,
            this.miShowInSkillBrowser,
            this.miShowInSkillExplorer,
            this.miMarkOwned,
            this.toolStripMenuItem1,
            this.miSubPlan,
            this.toolStripSeparator3,
            this.miChangeLevel,
            this.miRemoveFromPlan,
            this.toolStripSeparator2,
            this.miPlanGroups});
            this.cmsContextMenu.Name = "cmsContextMenu";
            this.cmsContextMenu.Size = new System.Drawing.Size(204, 220);
            this.cmsContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.cmsContextMenu_Opening);
            // 
            // miChangeNote
            // 
            this.miChangeNote.Name = "miChangeNote";
            this.miChangeNote.Size = new System.Drawing.Size(203, 22);
            this.miChangeNote.Text = "View/Change Note...";
            this.miChangeNote.Click += new System.EventHandler(this.miChangeNote_Click);
            // 
            // miChangePriority
            // 
            this.miChangePriority.Name = "miChangePriority";
            this.miChangePriority.Size = new System.Drawing.Size(203, 22);
            this.miChangePriority.Text = "Change Priority...";
            this.miChangePriority.Click += new System.EventHandler(this.miChangePriority_Click);
            // 
            // miShowInSkillBrowser
            // 
            this.miShowInSkillBrowser.Name = "miShowInSkillBrowser";
            this.miShowInSkillBrowser.Size = new System.Drawing.Size(203, 22);
            this.miShowInSkillBrowser.Text = "Show in Skill Browser...";
            this.miShowInSkillBrowser.Click += new System.EventHandler(this.miShowInSkillBrowser_Click);
            // 
            // miShowInSkillExplorer
            // 
            this.miShowInSkillExplorer.Name = "miShowInSkillExplorer";
            this.miShowInSkillExplorer.Size = new System.Drawing.Size(203, 22);
            this.miShowInSkillExplorer.Text = "Show in Skill Explorer";
            this.miShowInSkillExplorer.Click += new System.EventHandler(this.miShowInSkillExplorer_Click);
            // 
            // miMarkOwned
            // 
            this.miMarkOwned.Name = "miMarkOwned";
            this.miMarkOwned.Size = new System.Drawing.Size(203, 22);
            this.miMarkOwned.Text = "Mark as owned";
            this.miMarkOwned.Click += new System.EventHandler(this.miMarkOwned_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(200, 6);
            // 
            // miSubPlan
            // 
            this.miSubPlan.Name = "miSubPlan";
            this.miSubPlan.Size = new System.Drawing.Size(203, 22);
            this.miSubPlan.Text = "Create Sub-plan...";
            this.miSubPlan.Click += new System.EventHandler(this.miSubPlan_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(200, 6);
            // 
            // miChangeLevel
            // 
            this.miChangeLevel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miChangeTo0,
            this.miChangeTo1,
            this.miChangeTo2,
            this.miChangeTo3,
            this.miChangeTo4,
            this.miChangeTo5});
            this.miChangeLevel.Name = "miChangeLevel";
            this.miChangeLevel.Size = new System.Drawing.Size(203, 22);
            this.miChangeLevel.Text = "Change Planned Level...";
            // 
            // miChangeTo0
            // 
            this.miChangeTo0.Name = "miChangeTo0";
            this.miChangeTo0.Size = new System.Drawing.Size(124, 22);
            this.miChangeTo0.Tag = "0";
            this.miChangeTo0.Text = "Remove";
            this.miChangeTo0.Click += new System.EventHandler(this.miRemoveFromPlan_Click);
            // 
            // miChangeTo1
            // 
            this.miChangeTo1.Name = "miChangeTo1";
            this.miChangeTo1.Size = new System.Drawing.Size(124, 22);
            this.miChangeTo1.Tag = "1";
            this.miChangeTo1.Text = "Level 1";
            this.miChangeTo1.Click += new System.EventHandler(this.miChangeToN_Click);
            // 
            // miChangeTo2
            // 
            this.miChangeTo2.Name = "miChangeTo2";
            this.miChangeTo2.Size = new System.Drawing.Size(124, 22);
            this.miChangeTo2.Tag = "2";
            this.miChangeTo2.Text = "Level 2";
            this.miChangeTo2.Click += new System.EventHandler(this.miChangeToN_Click);
            // 
            // miChangeTo3
            // 
            this.miChangeTo3.Name = "miChangeTo3";
            this.miChangeTo3.Size = new System.Drawing.Size(124, 22);
            this.miChangeTo3.Tag = "3";
            this.miChangeTo3.Text = "Level 3";
            this.miChangeTo3.Click += new System.EventHandler(this.miChangeToN_Click);
            // 
            // miChangeTo4
            // 
            this.miChangeTo4.Name = "miChangeTo4";
            this.miChangeTo4.Size = new System.Drawing.Size(124, 22);
            this.miChangeTo4.Tag = "4";
            this.miChangeTo4.Text = "Level 4";
            this.miChangeTo4.Click += new System.EventHandler(this.miChangeToN_Click);
            // 
            // miChangeTo5
            // 
            this.miChangeTo5.Name = "miChangeTo5";
            this.miChangeTo5.Size = new System.Drawing.Size(124, 22);
            this.miChangeTo5.Tag = "5";
            this.miChangeTo5.Text = "Level 5";
            this.miChangeTo5.Click += new System.EventHandler(this.miChangeToN_Click);
            // 
            // miRemoveFromPlan
            // 
            this.miRemoveFromPlan.Name = "miRemoveFromPlan";
            this.miRemoveFromPlan.Size = new System.Drawing.Size(203, 22);
            this.miRemoveFromPlan.Text = "Remove from Plan...";
            this.miRemoveFromPlan.Click += new System.EventHandler(this.miRemoveFromPlan_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(200, 6);
            // 
            // miPlanGroups
            // 
            this.miPlanGroups.Name = "miPlanGroups";
            this.miPlanGroups.Size = new System.Drawing.Size(203, 22);
            this.miPlanGroups.Text = "Plan Groups";
            // 
            // sfdSave
            // 
            this.sfdSave.Filter = "EVEMon Plan Format (*.emp)|*.emp|XML Format (*.xml)|*.xml|Text Format (*.txt)|*.t" +
                "xt";
            this.sfdSave.Title = "Save Plan As...";
            // 
            // tmrAutoRefresh
            // 
            this.tmrAutoRefresh.Interval = 30000;
            this.tmrAutoRefresh.Tick += new System.EventHandler(this.tmrAutoRefresh_Tick);
            // 
            // tsPlan
            // 
            this.tsPlan.Dock = System.Windows.Forms.DockStyle.Left;
            this.tsPlan.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsPlan.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslMove,
            this.tsbMoveUp,
            this.tsbMoveDown,
            this.toolStripSeparator1,
            this.tslSort,
            this.tsSortLearning,
            this.tsSortPriorities,
            this.toolStripSeparator4,
            this.tsbToggleSkills,
            this.tsbToggleRemapping});
            this.tsPlan.Location = new System.Drawing.Point(0, 0);
            this.tsPlan.Name = "tsPlan";
            this.tsPlan.Size = new System.Drawing.Size(38, 558);
            this.tsPlan.TabIndex = 0;
            this.tsPlan.Text = "planToolStrip";
            // 
            // tslMove
            // 
            this.tslMove.Name = "tslMove";
            this.tslMove.Size = new System.Drawing.Size(35, 13);
            this.tslMove.Text = "Move:";
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(35, 6);
            this.toolStripSeparator1.Visible = false;
            // 
            // tslSort
            // 
            this.tslSort.Name = "tslSort";
            this.tslSort.Size = new System.Drawing.Size(35, 13);
            this.tslSort.Text = "Sort:";
            // 
            // tsSortLearning
            // 
            this.tsSortLearning.CheckOnClick = true;
            this.tsSortLearning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSortLearning.Image = global::EVEMon.Properties.Resources.text_letter_omega;
            this.tsSortLearning.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSortLearning.Name = "tsSortLearning";
            this.tsSortLearning.Size = new System.Drawing.Size(35, 20);
            this.tsSortLearning.Text = "Optimize learning skills order and put them ahead";
            // 
            // tsSortPriorities
            // 
            this.tsSortPriorities.CheckOnClick = true;
            this.tsSortPriorities.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSortPriorities.Image = global::EVEMon.Properties.Resources.text_list_numbers;
            this.tsSortPriorities.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSortPriorities.Name = "tsSortPriorities";
            this.tsSortPriorities.Size = new System.Drawing.Size(35, 20);
            this.tsSortPriorities.Text = "GroupByPriorities";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(35, 6);
            // 
            // tsbToggleSkills
            // 
            this.tsbToggleSkills.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToggleSkills.Image = ((System.Drawing.Image)(resources.GetObject("tsbToggleSkills.Image")));
            this.tsbToggleSkills.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToggleSkills.Name = "tsbToggleSkills";
            this.tsbToggleSkills.Size = new System.Drawing.Size(35, 20);
            this.tsbToggleSkills.Text = "tsbToggleSkills";
            this.tsbToggleSkills.ToolTipText = "Toggle Skill Pane";
            this.tsbToggleSkills.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // tsbToggleRemapping
            // 
            this.tsbToggleRemapping.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToggleRemapping.Image = ((System.Drawing.Image)(resources.GetObject("tsbToggleRemapping.Image")));
            this.tsbToggleRemapping.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToggleRemapping.Name = "tsbToggleRemapping";
            this.tsbToggleRemapping.Size = new System.Drawing.Size(35, 20);
            this.tsbToggleRemapping.Text = "tsbToggleRemapping";
            this.tsbToggleRemapping.ToolTipText = "Toggle remapping point (F9)";
            this.tsbToggleRemapping.Click += new System.EventHandler(this.tsbToggleRemapping_Click);
            // 
            // ilSkillDependency
            // 
            this.ilSkillDependency.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSkillDependency.ImageStream")));
            this.ilSkillDependency.TransparentColor = System.Drawing.Color.Transparent;
            this.ilSkillDependency.Images.SetKeyName(0, "redbutton.bmp");
            this.ilSkillDependency.Images.SetKeyName(1, "yellowbutton.bmp");
            this.ilSkillDependency.Images.SetKeyName(2, "greenbutton.bmp");
            this.ilSkillDependency.Images.SetKeyName(3, "shape_align_middle.png");
            // 
            // tmrSelect
            // 
            this.tmrSelect.Tick += new System.EventHandler(this.tmrSelect_Tick);
            // 
            // ilColumnsStates
            // 
            this.ilColumnsStates.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilColumnsStates.ImageStream")));
            this.ilColumnsStates.TransparentColor = System.Drawing.Color.Transparent;
            this.ilColumnsStates.Images.SetKeyName(0, "arrow_up.png");
            this.ilColumnsStates.Images.SetKeyName(1, "arrow_down.png");
            // 
            // pscPlan
            // 
            this.pscPlan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pscPlan.Location = new System.Drawing.Point(38, 0);
            this.pscPlan.Name = "pscPlan";
            // 
            // pscPlan.Panel1
            // 
            this.pscPlan.Panel1.Controls.Add(this.lvSkills);
            this.pscPlan.Panel1.Controls.Add(this.pHeader);
            // 
            // pscPlan.Panel2
            // 
            this.pscPlan.Panel2.Controls.Add(this.skillSelectControl);
            this.pscPlan.Panel2Collapsed = true;
            this.pscPlan.RememberDistanceKey = null;
            this.pscPlan.Size = new System.Drawing.Size(645, 558);
            this.pscPlan.SplitterDistance = 472;
            this.pscPlan.TabIndex = 13;
            // 
            // lvSkills
            // 
            this.lvSkills.AllowColumnReorder = true;
            this.lvSkills.AllowDrop = true;
            this.lvSkills.AllowRowReorder = true;
            this.lvSkills.ContextMenuStrip = this.cmsContextMenu;
            this.lvSkills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSkills.FullRowSelect = true;
            this.lvSkills.Location = new System.Drawing.Point(0, 21);
            this.lvSkills.Name = "lvSkills";
            this.lvSkills.ShowItemToolTips = true;
            this.lvSkills.Size = new System.Drawing.Size(645, 537);
            this.lvSkills.SmallImageList = this.ilColumnsStates;
            this.lvSkills.StateImageList = this.ilSkillDependency;
            this.lvSkills.TabIndex = 0;
            this.lvSkills.UseCompatibleStateImageBehavior = false;
            this.lvSkills.View = System.Windows.Forms.View.Details;
            this.lvSkills.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvSkills_MouseDoubleClick);
            this.lvSkills.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.lvSkills_ItemHover);
            this.lvSkills.SelectedIndexChanged += new System.EventHandler(this.lvSkills_SelectedIndexChanged);
            this.lvSkills.ListViewItemsDragged += new System.EventHandler<System.EventArgs>(this.lvSkills_ListViewItemsDragged);
            this.lvSkills.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvSkills_DragDrop);
            this.lvSkills.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvSkills_DragEnter);
            this.lvSkills.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvSkills_KeyUp);
            this.lvSkills.DragLeave += new System.EventHandler(this.lvSkills_DragLeave);
            this.lvSkills.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.lvSkills_ColumnReordered);
            this.lvSkills.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvSkills_KeyDown);
            this.lvSkills.DragOver += new System.Windows.Forms.DragEventHandler(this.lvSkills_DragOver);
            // 
            // pHeader
            // 
            this.pHeader.Controls.Add(this.llSuggestionLink);
            this.pHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pHeader.Location = new System.Drawing.Point(0, 0);
            this.pHeader.Name = "pHeader";
            this.pHeader.Size = new System.Drawing.Size(645, 21);
            this.pHeader.TabIndex = 1;
            // 
            // llSuggestionLink
            // 
            this.llSuggestionLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llSuggestionLink.Location = new System.Drawing.Point(464, -1);
            this.llSuggestionLink.Name = "llSuggestionLink";
            this.llSuggestionLink.Size = new System.Drawing.Size(181, 22);
            this.llSuggestionLink.TabIndex = 0;
            this.llSuggestionLink.TabStop = true;
            this.llSuggestionLink.Text = "Select Columns...";
            this.llSuggestionLink.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.llSuggestionLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llSuggestionLink_LinkClicked);
            // 
            // skillSelectControl
            // 
            this.skillSelectControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.skillSelectControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skillSelectControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.skillSelectControl.GrandCharacterInfo = null;
            this.skillSelectControl.Location = new System.Drawing.Point(0, 0);
            this.skillSelectControl.Margin = new System.Windows.Forms.Padding(2);
            this.skillSelectControl.Name = "skillSelectControl";
            this.skillSelectControl.Plan = null;
            this.skillSelectControl.Size = new System.Drawing.Size(96, 100);
            this.skillSelectControl.TabIndex = 12;
            // 
            // PlanOrderEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pscPlan);
            this.Controls.Add(this.tsPlan);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PlanOrderEditorControl";
            this.Size = new System.Drawing.Size(683, 558);
            this.Load += new System.EventHandler(this.PlanOrderEditorControl_Load);
            this.cmsContextMenu.ResumeLayout(false);
            this.tsPlan.ResumeLayout(false);
            this.tsPlan.PerformLayout();
            this.pscPlan.Panel1.ResumeLayout(false);
            this.pscPlan.Panel2.ResumeLayout(false);
            this.pscPlan.ResumeLayout(false);
            this.pHeader.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EVEMon.SkillPlanner.DraggableListView lvSkills;
        private System.Windows.Forms.ContextMenuStrip cmsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem miRemoveFromPlan;
        private System.Windows.Forms.SaveFileDialog sfdSave;
        private System.Windows.Forms.Timer tmrAutoRefresh;
        private System.Windows.Forms.ToolStrip tsPlan;
        private System.Windows.Forms.ToolStripLabel tslMove;
        private System.Windows.Forms.ToolStripButton tsbMoveUp;
        private System.Windows.Forms.ToolStripButton tsbMoveDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel pHeader;
        private System.Windows.Forms.LinkLabel llSuggestionLink;
        private System.Windows.Forms.ToolStripMenuItem miChangeNote;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripLabel tslSort;
        private System.Windows.Forms.ImageList ilSkillDependency;
        private System.Windows.Forms.ToolStripMenuItem miShowInSkillBrowser;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miPlanGroups;
        private System.Windows.Forms.ToolStripMenuItem miSubPlan;
        private System.Windows.Forms.ToolStripMenuItem miChangePriority;
        private System.Windows.Forms.ToolStripMenuItem miMarkOwned;
        private System.Windows.Forms.ToolStripMenuItem miShowInSkillExplorer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem miChangeLevel;
        private System.Windows.Forms.ToolStripMenuItem miChangeTo0;
        private System.Windows.Forms.ToolStripMenuItem miChangeTo1;
        private System.Windows.Forms.ToolStripMenuItem miChangeTo2;
        private System.Windows.Forms.ToolStripMenuItem miChangeTo3;
        private System.Windows.Forms.ToolStripMenuItem miChangeTo4;
        private System.Windows.Forms.ToolStripMenuItem miChangeTo5;
        private SkillSelectControl skillSelectControl;
        private PersistentSplitContainer pscPlan;
        private System.Windows.Forms.ToolStripButton tsbToggleSkills;
        private System.Windows.Forms.Timer tmrSelect;
        private System.Windows.Forms.ToolStripButton tsbToggleRemapping;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ImageList ilColumnsStates;
        private System.Windows.Forms.ToolStripButton tsSortLearning;
        private System.Windows.Forms.ToolStripButton tsSortPriorities;
    }
}
