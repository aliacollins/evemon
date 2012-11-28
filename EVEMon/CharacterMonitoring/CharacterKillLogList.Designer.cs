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
            this.lbKillLog = new EVEMon.Common.Controls.NoFlickerListBox();
            this.lvKillLog = new System.Windows.Forms.ListView();
            this.chDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCorporation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAlliance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chFaction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
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
            this.lbKillLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbKillLogs_MouseDown);
            this.lbKillLog.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.lbKillLogs_MouseWheel);
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
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "arrow_up.png");
            this.ilIcons.Images.SetKeyName(1, "arrow_down.png");
            this.ilIcons.Images.SetKeyName(2, "16x16Transparant.png");
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
    }
}
