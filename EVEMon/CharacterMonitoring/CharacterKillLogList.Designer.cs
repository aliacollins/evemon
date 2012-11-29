namespace EVEMon.CharacterMonitoring
{
    partial class CharacterKillLogList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterKillLogList));
            this.noKillLogLabel = new System.Windows.Forms.Label();
            this.lvKillLog = new System.Windows.Forms.ListView();
            this.chDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCorporation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlliance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFaction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiShowDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCopyKillInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.lbKillLog = new EVEMon.Common.Controls.NoFlickerListBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // noKillLogLabel
            // 
            this.noKillLogLabel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.noKillLogLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noKillLogLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.noKillLogLabel.Location = new System.Drawing.Point(0, 0);
            this.noKillLogLabel.Margin = new System.Windows.Forms.Padding(0);
            this.noKillLogLabel.Name = "noKillLogLabel";
            this.noKillLogLabel.Size = new System.Drawing.Size(324, 382);
            this.noKillLogLabel.TabIndex = 5;
            this.noKillLogLabel.Text = "Combat log information not available.";
            this.noKillLogLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvKillLog
            // 
            this.lvKillLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvKillLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chDate,
            this.chType,
            this.chName,
            this.chCorporation,
            this.chAlliance,
            this.chFaction});
            this.lvKillLog.ContextMenuStrip = this.contextMenuStrip;
            this.lvKillLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvKillLog.FullRowSelect = true;
            this.lvKillLog.HideSelection = false;
            this.lvKillLog.Location = new System.Drawing.Point(0, 0);
            this.lvKillLog.MultiSelect = false;
            this.lvKillLog.Name = "lvKillLog";
            this.lvKillLog.Size = new System.Drawing.Size(324, 382);
            this.lvKillLog.SmallImageList = this.ilIcons;
            this.lvKillLog.TabIndex = 7;
            this.lvKillLog.UseCompatibleStateImageBehavior = false;
            this.lvKillLog.View = System.Windows.Forms.View.Details;
            this.lvKillLog.DoubleClick += new System.EventHandler(this.lvKillLog_DoubleClick);
            // 
            // chDate
            // 
            this.chDate.Text = "Date";
            this.chDate.Width = 106;
            // 
            // chType
            // 
            this.chType.Text = "Type";
            this.chType.Width = 100;
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 99;
            // 
            // chCorporation
            // 
            this.chCorporation.Text = "Corporation";
            // 
            // chAlliance
            // 
            this.chAlliance.Text = "Alliance";
            // 
            // chFaction
            // 
            this.chFaction.Text = "Faction";
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiShowDetails,
            this.toolStripSeparator,
            this.tsmiCopyKillInfo});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(147, 54);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // tsmiShowDetails
            // 
            this.tsmiShowDetails.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tsmiShowDetails.Name = "tsmiShowDetails";
            this.tsmiShowDetails.Size = new System.Drawing.Size(146, 22);
            this.tsmiShowDetails.Text = "Show Details";
            this.tsmiShowDetails.Click += new System.EventHandler(this.tsmiShowDetails_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
            // 
            // tsmiCopyKillInfo
            // 
            this.tsmiCopyKillInfo.Name = "tsmiCopyKillInfo";
            this.tsmiCopyKillInfo.Size = new System.Drawing.Size(146, 22);
            this.tsmiCopyKillInfo.Text = "Copy Kill info";
            this.tsmiCopyKillInfo.Click += new System.EventHandler(this.tsmiCopyKillInfo_Click);
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "arrow_up.png");
            this.ilIcons.Images.SetKeyName(1, "arrow_down.png");
            this.ilIcons.Images.SetKeyName(2, "16x16Transparant.png");
            // 
            // lbKillLog
            // 
            this.lbKillLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbKillLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbKillLog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lbKillLog.FormattingEnabled = true;
            this.lbKillLog.IntegralHeight = false;
            this.lbKillLog.ItemHeight = 15;
            this.lbKillLog.Location = new System.Drawing.Point(0, 0);
            this.lbKillLog.Margin = new System.Windows.Forms.Padding(0);
            this.lbKillLog.Name = "lbKillLog";
            this.lbKillLog.Size = new System.Drawing.Size(324, 382);
            this.lbKillLog.TabIndex = 6;
            this.lbKillLog.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbKillLog_DrawItem);
            this.lbKillLog.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lbKillLog_MeasureItem);
            this.lbKillLog.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbKillLog_MouseDoubleClick);
            this.lbKillLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbKillLogs_MouseDown);
            this.lbKillLog.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbKillLog_MouseMove);
            this.lbKillLog.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.lbKillLogs_MouseWheel);
            // 
            // CharacterKillLogList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.noKillLogLabel);
            this.Controls.Add(this.lbKillLog);
            this.Controls.Add(this.lvKillLog);
            this.Name = "CharacterKillLogList";
            this.Size = new System.Drawing.Size(324, 382);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label noKillLogLabel;
        private Common.Controls.NoFlickerListBox lbKillLog;
        private System.Windows.Forms.ListView lvKillLog;
        private System.Windows.Forms.ColumnHeader chDate;
        private System.Windows.Forms.ColumnHeader chType;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chCorporation;
        private System.Windows.Forms.ColumnHeader chAlliance;
        private System.Windows.Forms.ColumnHeader chFaction;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowDetails;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyKillInfo;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
