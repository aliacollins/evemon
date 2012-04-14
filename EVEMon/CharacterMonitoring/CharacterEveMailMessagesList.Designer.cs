namespace EVEMon.CharacterMonitoring
{
    partial class CharacterEveMailMessagesList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharacterEveMailMessagesList));
            this.ilIcons = new System.Windows.Forms.ImageList(this.components);
            this.noEVEMailMessagesLabel = new System.Windows.Forms.Label();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mailReadLocal = new System.Windows.Forms.ToolStripMenuItem();
            this.mailOpenExternal = new System.Windows.Forms.ToolStripMenuItem();
            this.mailGateRead = new System.Windows.Forms.ToolStripMenuItem();
            this.mailGateSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mailGateReply = new System.Windows.Forms.ToolStripMenuItem();
            this.mailGateReplyAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mailGateForward = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerMailMessages = new System.Windows.Forms.SplitContainer();
            this.lvMailMessages = new System.Windows.Forms.ListView();
            this.chSenderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSentDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chToCharacterIDs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chToCorpOrAlliance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chToListID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eveMailReadingPane = new EVEMon.CharacterMonitoring.ReadingPane();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMailMessages)).BeginInit();
            this.splitContainerMailMessages.Panel1.SuspendLayout();
            this.splitContainerMailMessages.Panel2.SuspendLayout();
            this.splitContainerMailMessages.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilIcons
            // 
            this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
            this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilIcons.Images.SetKeyName(0, "arrow_up.png");
            this.ilIcons.Images.SetKeyName(1, "arrow_down.png");
            this.ilIcons.Images.SetKeyName(2, "16x16Transparant.png");
            // 
            // noEVEMailMessagesLabel
            // 
            this.noEVEMailMessagesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noEVEMailMessagesLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.noEVEMailMessagesLabel.Location = new System.Drawing.Point(0, 0);
            this.noEVEMailMessagesLabel.Name = "noEVEMailMessagesLabel";
            this.noEVEMailMessagesLabel.Size = new System.Drawing.Size(454, 434);
            this.noEVEMailMessagesLabel.TabIndex = 1;
            this.noEVEMailMessagesLabel.Text = "No EVE mail messages are available.";
            this.noEVEMailMessagesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mailReadLocal,
            this.mailOpenExternal});
            this.contextMenu.Name = "mailListContextMenu";
            this.contextMenu.Size = new System.Drawing.Size(180, 48);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // mailReadLocal
            // 
            this.mailReadLocal.Name = "mailReadLocal";
            this.mailReadLocal.Size = new System.Drawing.Size(179, 22);
            this.mailReadLocal.Text = "Read";
            this.mailReadLocal.Click += new System.EventHandler(this.mailReadLocal_Click);
            // 
            // mailOpenExternal
            // 
            this.mailOpenExternal.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mailGateRead,
            this.mailGateSep1,
            this.mailGateReply,
            this.mailGateReplyAll,
            this.mailGateForward});
            this.mailOpenExternal.Name = "mailOpenExternal";
            this.mailOpenExternal.Size = new System.Drawing.Size(179, 22);
            this.mailOpenExternal.Text = "Open in EVE Gate to";
            // 
            // mailGateRead
            // 
            this.mailGateRead.Name = "mailGateRead";
            this.mailGateRead.Size = new System.Drawing.Size(118, 22);
            this.mailGateRead.Text = "Read";
            this.mailGateRead.Click += new System.EventHandler(this.mailGateRead_Click);
            // 
            // mailGateSep1
            // 
            this.mailGateSep1.Name = "mailGateSep1";
            this.mailGateSep1.Size = new System.Drawing.Size(115, 6);
            // 
            // mailGateReply
            // 
            this.mailGateReply.Name = "mailGateReply";
            this.mailGateReply.Size = new System.Drawing.Size(118, 22);
            this.mailGateReply.Text = "Reply";
            this.mailGateReply.Click += new System.EventHandler(this.mailGateReply_Click);
            // 
            // mailGateReplyAll
            // 
            this.mailGateReplyAll.Name = "mailGateReplyAll";
            this.mailGateReplyAll.Size = new System.Drawing.Size(118, 22);
            this.mailGateReplyAll.Text = "Reply all";
            this.mailGateReplyAll.Click += new System.EventHandler(this.mailGateReplyAll_Click);
            // 
            // mailGateForward
            // 
            this.mailGateForward.Name = "mailGateForward";
            this.mailGateForward.Size = new System.Drawing.Size(118, 22);
            this.mailGateForward.Text = "Forward";
            this.mailGateForward.Click += new System.EventHandler(this.mailGateForward_Click);
            // 
            // splitContainerMailMessages
            // 
            this.splitContainerMailMessages.BackColor = System.Drawing.SystemColors.ControlDark;
            this.splitContainerMailMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMailMessages.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMailMessages.Name = "splitContainerMailMessages";
            this.splitContainerMailMessages.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMailMessages.Panel1
            // 
            this.splitContainerMailMessages.Panel1.Controls.Add(this.lvMailMessages);
            // 
            // splitContainerMailMessages.Panel2
            // 
            this.splitContainerMailMessages.Panel2.Controls.Add(this.eveMailReadingPane);
            this.splitContainerMailMessages.Size = new System.Drawing.Size(454, 434);
            this.splitContainerMailMessages.SplitterDistance = 288;
            this.splitContainerMailMessages.SplitterWidth = 6;
            this.splitContainerMailMessages.TabIndex = 2;
            // 
            // lvMailMessages
            // 
            this.lvMailMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvMailMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chSenderName,
            this.chTitle,
            this.chSentDate,
            this.chToCharacterIDs,
            this.chToCorpOrAlliance,
            this.chToListID});
            this.lvMailMessages.ContextMenuStrip = this.contextMenu;
            this.lvMailMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMailMessages.FullRowSelect = true;
            this.lvMailMessages.HideSelection = false;
            this.lvMailMessages.Location = new System.Drawing.Point(0, 0);
            this.lvMailMessages.MultiSelect = false;
            this.lvMailMessages.Name = "lvMailMessages";
            this.lvMailMessages.Size = new System.Drawing.Size(454, 288);
            this.lvMailMessages.SmallImageList = this.ilIcons;
            this.lvMailMessages.TabIndex = 0;
            this.lvMailMessages.UseCompatibleStateImageBehavior = false;
            this.lvMailMessages.View = System.Windows.Forms.View.Details;
            this.lvMailMessages.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvMailMessages_ColumnClick);
            this.lvMailMessages.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.lvMailMessages_ColumnReordered);
            this.lvMailMessages.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.lvMailMessages_ColumnWidthChanged);
            this.lvMailMessages.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvMailMessages_ItemSelectionChanged);
            this.lvMailMessages.DoubleClick += new System.EventHandler(this.lvMailMessages_DoubleClick);
            // 
            // chSenderName
            // 
            this.chSenderName.Text = "From";
            // 
            // chTitle
            // 
            this.chTitle.Text = "Subject";
            this.chTitle.Width = 121;
            // 
            // chSentDate
            // 
            this.chSentDate.Text = "Received";
            this.chSentDate.Width = 90;
            // 
            // chToCharacterIDs
            // 
            this.chToCharacterIDs.Text = "To";
            // 
            // chToCorpOrAlliance
            // 
            this.chToCorpOrAlliance.Text = "To Corp Or Alliance";
            // 
            // chToListID
            // 
            this.chToListID.Text = "To Mailing List";
            // 
            // eveMailReadingPane
            // 
            this.eveMailReadingPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eveMailReadingPane.Location = new System.Drawing.Point(0, 0);
            this.eveMailReadingPane.Name = "eveMailReadingPane";
            this.eveMailReadingPane.Size = new System.Drawing.Size(454, 140);
            this.eveMailReadingPane.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // CharacterEveMailMessagesList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerMailMessages);
            this.Controls.Add(this.noEVEMailMessagesLabel);
            this.Name = "CharacterEveMailMessagesList";
            this.Size = new System.Drawing.Size(454, 434);
            this.contextMenu.ResumeLayout(false);
            this.splitContainerMailMessages.Panel1.ResumeLayout(false);
            this.splitContainerMailMessages.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMailMessages)).EndInit();
            this.splitContainerMailMessages.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label noEVEMailMessagesLabel;
        private System.Windows.Forms.ImageList ilIcons;
        private System.Windows.Forms.ColumnHeader chSenderName;
        private System.Windows.Forms.ColumnHeader chTitle;
        private System.Windows.Forms.ColumnHeader chSentDate;
        private System.Windows.Forms.ColumnHeader chToCorpOrAlliance;
        private System.Windows.Forms.ColumnHeader chToCharacterIDs;
        private System.Windows.Forms.ColumnHeader chToListID;
        private System.Windows.Forms.SplitContainer splitContainerMailMessages;
        private System.Windows.Forms.ListView lvMailMessages;
        private System.Windows.Forms.Timer timer;
        private ReadingPane eveMailReadingPane;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem mailReadLocal;
        private System.Windows.Forms.ToolStripMenuItem mailOpenExternal;
        private System.Windows.Forms.ToolStripMenuItem mailGateRead;
        private System.Windows.Forms.ToolStripSeparator mailGateSep1;
        private System.Windows.Forms.ToolStripMenuItem mailGateReply;
        private System.Windows.Forms.ToolStripMenuItem mailGateReplyAll;
        private System.Windows.Forms.ToolStripMenuItem mailGateForward;
    }
}
