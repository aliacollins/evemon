namespace EVEMon.SkillPlanner
{
    partial class EFTLoadout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EFTLoadout));
            this.TopSplitContainer = new System.Windows.Forms.SplitContainer();
            this.PasteTextBox = new System.Windows.Forms.RichTextBox();
            this.ExplantionLabel = new System.Windows.Forms.Label();
            this.ResultsTreeView = new System.Windows.Forms.TreeView();
            this.RightClickContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowInBrowserMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.PlanedLabel = new System.Windows.Forms.Label();
            this.AddToPlanButton = new System.Windows.Forms.Button();
            this.TrainingTimeLabel = new System.Windows.Forms.Label();
            this.TrainTimeLabel = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.TopSplitContainer.Panel1.SuspendLayout();
            this.TopSplitContainer.Panel2.SuspendLayout();
            this.TopSplitContainer.SuspendLayout();
            this.RightClickContextMenuStrip.SuspendLayout();
            this.BottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopSplitContainer
            // 
            this.TopSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.TopSplitContainer.Name = "TopSplitContainer";
            // 
            // TopSplitContainer.Panel1
            // 
            this.TopSplitContainer.Panel1.Controls.Add(this.PasteTextBox);
            this.TopSplitContainer.Panel1.Controls.Add(this.ExplantionLabel);
            // 
            // TopSplitContainer.Panel2
            // 
            this.TopSplitContainer.Panel2.Controls.Add(this.ResultsTreeView);
            this.TopSplitContainer.Size = new System.Drawing.Size(747, 407);
            this.TopSplitContainer.SplitterDistance = 249;
            this.TopSplitContainer.TabIndex = 1;
            // 
            // PasteTextBox
            // 
            this.PasteTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PasteTextBox.Location = new System.Drawing.Point(0, 23);
            this.PasteTextBox.Name = "PasteTextBox";
            this.PasteTextBox.Size = new System.Drawing.Size(249, 384);
            this.PasteTextBox.TabIndex = 0;
            this.PasteTextBox.Text = "";
            this.PasteTextBox.WordWrap = false;
            this.PasteTextBox.TextChanged += new System.EventHandler(this.tbEFTLoadout_TextChanged);
            // 
            // ExplantionLabel
            // 
            this.ExplantionLabel.AutoSize = true;
            this.ExplantionLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ExplantionLabel.Location = new System.Drawing.Point(0, 0);
            this.ExplantionLabel.Name = "ExplantionLabel";
            this.ExplantionLabel.Padding = new System.Windows.Forms.Padding(5);
            this.ExplantionLabel.Size = new System.Drawing.Size(208, 23);
            this.ExplantionLabel.TabIndex = 1;
            this.ExplantionLabel.Text = "Paste your EFT loadout in the box below";
            // 
            // ResultsTreeView
            // 
            this.ResultsTreeView.ContextMenuStrip = this.RightClickContextMenuStrip;
            this.ResultsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultsTreeView.Location = new System.Drawing.Point(0, 0);
            this.ResultsTreeView.Name = "ResultsTreeView";
            this.ResultsTreeView.Size = new System.Drawing.Size(494, 407);
            this.ResultsTreeView.TabIndex = 1;
            this.ResultsTreeView.DoubleClick += new System.EventHandler(this.tvLoadout_DoubleClick);
            this.ResultsTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvLoadout_MouseUp);
            // 
            // RightClickContextMenuStrip
            // 
            this.RightClickContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowInBrowserMenuItem});
            this.RightClickContextMenuStrip.Name = "cmNode";
            this.RightClickContextMenuStrip.Size = new System.Drawing.Size(198, 26);
            // 
            // ShowInBrowserMenuItem
            // 
            this.ShowInBrowserMenuItem.Name = "ShowInBrowserMenuItem";
            this.ShowInBrowserMenuItem.Size = new System.Drawing.Size(197, 22);
            this.ShowInBrowserMenuItem.Text = "Show Item In Browser...";
            this.ShowInBrowserMenuItem.Click += new System.EventHandler(this.tvLoadout_DoubleClick);
            // 
            // BottomPanel
            // 
            this.BottomPanel.Controls.Add(this.PlanedLabel);
            this.BottomPanel.Controls.Add(this.AddToPlanButton);
            this.BottomPanel.Controls.Add(this.TrainingTimeLabel);
            this.BottomPanel.Controls.Add(this.TrainTimeLabel);
            this.BottomPanel.Controls.Add(this.CancelButton);
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPanel.Location = new System.Drawing.Point(0, 407);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(747, 44);
            this.BottomPanel.TabIndex = 2;
            // 
            // PlanedLabel
            // 
            this.PlanedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PlanedLabel.AutoSize = true;
            this.PlanedLabel.Location = new System.Drawing.Point(191, 14);
            this.PlanedLabel.Name = "PlanedLabel";
            this.PlanedLabel.Size = new System.Drawing.Size(0, 13);
            this.PlanedLabel.TabIndex = 34;
            // 
            // AddToPlanButton
            // 
            this.AddToPlanButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddToPlanButton.Enabled = false;
            this.AddToPlanButton.Location = new System.Drawing.Point(579, 9);
            this.AddToPlanButton.Name = "AddToPlanButton";
            this.AddToPlanButton.Size = new System.Drawing.Size(75, 23);
            this.AddToPlanButton.TabIndex = 2;
            this.AddToPlanButton.Text = "Add To Plan";
            this.AddToPlanButton.UseVisualStyleBackColor = true;
            this.AddToPlanButton.Click += new System.EventHandler(this.btnPlan_Click);
            // 
            // TrainingTimeLabel
            // 
            this.TrainingTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TrainingTimeLabel.AutoSize = true;
            this.TrainingTimeLabel.Location = new System.Drawing.Point(12, 14);
            this.TrainingTimeLabel.Name = "TrainingTimeLabel";
            this.TrainingTimeLabel.Size = new System.Drawing.Size(173, 13);
            this.TrainingTimeLabel.TabIndex = 31;
            this.TrainingTimeLabel.Text = "Training Time for selected loadout: ";
            // 
            // TrainTimeLabel
            // 
            this.TrainTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TrainTimeLabel.AutoSize = true;
            this.TrainTimeLabel.Location = new System.Drawing.Point(191, 14);
            this.TrainTimeLabel.Name = "TrainTimeLabel";
            this.TrainTimeLabel.Size = new System.Drawing.Size(35, 13);
            this.TrainTimeLabel.TabIndex = 32;
            this.TrainTimeLabel.Text = "label2";
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(660, 9);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Close";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // EFTLoadout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 451);
            this.Controls.Add(this.TopSplitContainer);
            this.Controls.Add(this.BottomPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EFTLoadout";
            this.Text = "EFT import";
            this.TopSplitContainer.Panel1.ResumeLayout(false);
            this.TopSplitContainer.Panel1.PerformLayout();
            this.TopSplitContainer.Panel2.ResumeLayout(false);
            this.TopSplitContainer.ResumeLayout(false);
            this.RightClickContextMenuStrip.ResumeLayout(false);
            this.BottomPanel.ResumeLayout(false);
            this.BottomPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer TopSplitContainer;
        private System.Windows.Forms.TreeView ResultsTreeView;
        private System.Windows.Forms.Panel BottomPanel;
        private System.Windows.Forms.Button AddToPlanButton;
        private System.Windows.Forms.Label TrainingTimeLabel;
        private System.Windows.Forms.Label TrainTimeLabel;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label PlanedLabel;
        private System.Windows.Forms.Label ExplantionLabel;
        private System.Windows.Forms.ContextMenuStrip RightClickContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ShowInBrowserMenuItem;
        private System.Windows.Forms.RichTextBox PasteTextBox;
    }
}
