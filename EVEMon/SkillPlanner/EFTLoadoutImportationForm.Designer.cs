namespace EVEMon.SkillPlanner
{
    partial class EFTLoadoutImportationForm
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
            this.topSplitContainer = new EVEMon.Controls.PersistentSplitContainer();
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
            this.CloseButton = new System.Windows.Forms.Button();
            this.topSplitContainer.Panel1.SuspendLayout();
            this.topSplitContainer.Panel2.SuspendLayout();
            this.topSplitContainer.SuspendLayout();
            this.RightClickContextMenuStrip.SuspendLayout();
            this.BottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // topSplitContainer
            // 
            this.topSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.topSplitContainer.Name = "topSplitContainer";
            // 
            // topSplitContainer.Panel1
            // 
            this.topSplitContainer.Panel1.Controls.Add(this.PasteTextBox);
            this.topSplitContainer.Panel1.Controls.Add(this.ExplantionLabel);
            // 
            // topSplitContainer.Panel2
            // 
            this.topSplitContainer.Panel2.Controls.Add(this.ResultsTreeView);
            this.topSplitContainer.Size = new System.Drawing.Size(747, 401);
            this.topSplitContainer.SplitterDistance = 249;
            this.topSplitContainer.TabIndex = 1;
            // 
            // PasteTextBox
            // 
            this.PasteTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PasteTextBox.Location = new System.Drawing.Point(0, 23);
            this.PasteTextBox.Name = "PasteTextBox";
            this.PasteTextBox.Size = new System.Drawing.Size(249, 378);
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
            this.ExplantionLabel.Size = new System.Drawing.Size(211, 23);
            this.ExplantionLabel.TabIndex = 1;
            this.ExplantionLabel.Text = "Paste your EFT loadout in the box below";
            // 
            // ResultsTreeView
            // 
            this.ResultsTreeView.ContextMenuStrip = this.RightClickContextMenuStrip;
            this.ResultsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResultsTreeView.Location = new System.Drawing.Point(0, 0);
            this.ResultsTreeView.Name = "ResultsTreeView";
            this.ResultsTreeView.Size = new System.Drawing.Size(494, 401);
            this.ResultsTreeView.TabIndex = 1;
            this.ResultsTreeView.DoubleClick += new System.EventHandler(this.tvLoadout_DoubleClick);
            this.ResultsTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvLoadout_MouseUp);
            // 
            // RightClickContextMenuStrip
            // 
            this.RightClickContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowInBrowserMenuItem});
            this.RightClickContextMenuStrip.Name = "cmNode";
            this.RightClickContextMenuStrip.Size = new System.Drawing.Size(204, 26);
            // 
            // ShowInBrowserMenuItem
            // 
            this.ShowInBrowserMenuItem.Name = "ShowInBrowserMenuItem";
            this.ShowInBrowserMenuItem.Size = new System.Drawing.Size(203, 22);
            this.ShowInBrowserMenuItem.Text = "Show Item In Browser...";
            this.ShowInBrowserMenuItem.Click += new System.EventHandler(this.tvLoadout_DoubleClick);
            // 
            // BottomPanel
            // 
            this.BottomPanel.Controls.Add(this.PlanedLabel);
            this.BottomPanel.Controls.Add(this.AddToPlanButton);
            this.BottomPanel.Controls.Add(this.TrainingTimeLabel);
            this.BottomPanel.Controls.Add(this.TrainTimeLabel);
            this.BottomPanel.Controls.Add(this.CloseButton);
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomPanel.Location = new System.Drawing.Point(0, 401);
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
            this.TrainingTimeLabel.Size = new System.Drawing.Size(176, 13);
            this.TrainingTimeLabel.TabIndex = 31;
            this.TrainingTimeLabel.Text = "Training Time for selected loadout: ";
            // 
            // TrainTimeLabel
            // 
            this.TrainTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TrainTimeLabel.AutoSize = true;
            this.TrainTimeLabel.Location = new System.Drawing.Point(191, 14);
            this.TrainTimeLabel.Name = "TrainTimeLabel";
            this.TrainTimeLabel.Size = new System.Drawing.Size(115, 13);
            this.TrainTimeLabel.TabIndex = 32;
            this.TrainTimeLabel.Text = "All skills already known";
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(660, 9);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // EFTLoadoutImportationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 445);
            this.Controls.Add(this.topSplitContainer);
            this.Controls.Add(this.BottomPanel);
            this.Name = "EFTLoadoutImportationForm";
            this.Text = "EFT import";
            this.topSplitContainer.Panel1.ResumeLayout(false);
            this.topSplitContainer.Panel1.PerformLayout();
            this.topSplitContainer.Panel2.ResumeLayout(false);
            this.topSplitContainer.ResumeLayout(false);
            this.RightClickContextMenuStrip.ResumeLayout(false);
            this.BottomPanel.ResumeLayout(false);
            this.BottomPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private EVEMon.Controls.PersistentSplitContainer topSplitContainer;
        private System.Windows.Forms.TreeView ResultsTreeView;
        private System.Windows.Forms.Panel BottomPanel;
        private System.Windows.Forms.Button AddToPlanButton;
        private System.Windows.Forms.Label TrainingTimeLabel;
        private System.Windows.Forms.Label TrainTimeLabel;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label PlanedLabel;
        private System.Windows.Forms.Label ExplantionLabel;
        private System.Windows.Forms.ContextMenuStrip RightClickContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ShowInBrowserMenuItem;
        private System.Windows.Forms.RichTextBox PasteTextBox;
    }
}
