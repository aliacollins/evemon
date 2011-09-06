using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Notifications;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    public partial class MainWindowEveMailMessagesList : UserControl, IGroupingListView
    {
        #region Fields

        private readonly List<EveMailMessagesColumnSettings> m_columns = new List<EveMailMessagesColumnSettings>();
        private readonly List<EveMailMessage> m_list = new List<EveMailMessage>();

        private EVEMailMessagesGrouping m_grouping;
        private EveMailMessagesColumn m_sortCriteria;
        private ReadingPanePositioning m_panePosition;

        private string m_textFilter = String.Empty;
        private bool m_sortAscending;
        private bool m_columnsChanged;
        private bool m_isUpdatingColumns;
        private bool m_init;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowEveMailMessagesList()
        {
            InitializeComponent();

            eveMailReadingPane.HidePane();
            splitContainerMailMessages.Visible = false;
            lvMailMessages.Visible = false;
            lvMailMessages.AllowColumnReorder = true;
            lvMailMessages.Columns.Clear();

            noEVEMailMessagesLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);

            ListViewHelper.EnableDoubleBuffer(lvMailMessages);

            EveMonClient.TimerTick += EveMonClient_TimerTick;
            EveMonClient.CharacterEVEMailMessagesUpdated += EveMonClient_CharacterEVEMailMessagesUpdated;
            EveMonClient.CharacterEVEMailBodyDownloaded += EveMonClient_CharacterEVEMailBodyDownloaded;
            EveMonClient.NotificationSent += EveMonClient_NotificationSent;
            Disposed += OnDisposed;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the character associated with this monitor.
        /// </summary>
        public Character Character { get; set; }

        /// <summary>
        /// Gets or sets the text filter.
        /// </summary>
        public string TextFilter
        {
            get { return m_textFilter; }
            set
            {
                m_textFilter = value;
                if (m_init)
                    UpdateColumns();
            }
        }

        /// <summary>
        /// Gets or sets the grouping mode.
        /// </summary>
        public Enum Grouping
        {
            get { return m_grouping; }
            set
            {
                m_grouping = (EVEMailMessagesGrouping)value;
                if (m_init)
                    UpdateColumns();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadingPanePositioning PanePosition
        {
            get { return m_panePosition; }
            set
            {
                m_panePosition = value;
                UpdatePanePosition();
            }
        }

        /// <summary>
        /// Gets or sets the enumeration of EVE mail messages to display.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<EveMailMessage> EVEMailMessages
        {
            get { return m_list; }
            set
            {
                m_list.Clear();
                if (value == null)
                    return;

                m_list.AddRange(value);
            }
        }

        /// <summary>
        /// Gets or sets the settings used for columns.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IEnumerable<EveMailMessagesColumnSettings> Columns
        {
            get
            {
                // Add the visible columns; matching the display order
                List<EveMailMessagesColumnSettings> newColumns = new List<EveMailMessagesColumnSettings>();
                foreach (ColumnHeader header in lvMailMessages.Columns.Cast<ColumnHeader>().OrderBy(x => x.DisplayIndex))
                {
                    EveMailMessagesColumnSettings columnSetting =
                        m_columns.First(x => x.Column == (EveMailMessagesColumn)header.Tag);
                    if (columnSetting.Width != -1)
                        columnSetting.Width = header.Width;

                    newColumns.Add(columnSetting);
                }

                // Then add the other columns
                newColumns.AddRange(m_columns.Where(x => !x.Visible));

                return newColumns;
            }
            set
            {
                m_columns.Clear();
                if (value != null)
                    m_columns.AddRange(value.Select(x => x.Clone()));

                if (m_init)
                    UpdateColumns();
            }
        }

        #endregion


        # region Inherited Events

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.TimerTick -= EveMonClient_TimerTick;
            EveMonClient.CharacterEVEMailMessagesUpdated -= EveMonClient_CharacterEVEMailMessagesUpdated;
            EveMonClient.CharacterEVEMailBodyDownloaded -= EveMonClient_CharacterEVEMailBodyDownloaded;
            EveMonClient.NotificationSent -= EveMonClient_NotificationSent;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// When the control becomes visible again, we update the content.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (DesignMode || this.IsDesignModeHosted() || Character == null)
                return;

            base.OnVisibleChanged(e);

            if (!Visible)
                return;

            // Prevents the properties to call UpdateColumns() till we set all properties
            m_init = false;

            CCPCharacter ccpCharacter = Character as CCPCharacter;
            EVEMailMessages = (ccpCharacter == null ? null : ccpCharacter.EVEMailMessages);
            Columns = Settings.UI.MainWindow.EVEMailMessages.Columns;
            Grouping = (Character == null ? EVEMailMessagesGrouping.State : Character.UISettings.EVEMailMessagesGroupBy);
            PanePosition = Settings.UI.MainWindow.EVEMailMessages.ReadingPanePosition;

            UpdateColumns();
            m_init = true;

            UpdateContent();
        }

        # endregion


        #region Update Methods

        /// <summary>
        /// Updates the columns.
        /// </summary>
        private void UpdateColumns()
        {
            lvMailMessages.BeginUpdate();
            m_isUpdatingColumns = true;

            try
            {
                lvMailMessages.Columns.Clear();

                foreach (EveMailMessagesColumnSettings column in m_columns.Where(x => x.Visible))
                {
                    ColumnHeader header = lvMailMessages.Columns.Add(column.Column.GetHeader(), column.Column.GetHeader(),
                                                                     column.Width);
                    header.Tag = column.Column;
                }

                // We update the content
                UpdateContent();

                // Force the auto-resize of the columns with -1 width
                ColumnHeaderAutoResizeStyle resizeStyle = (lvMailMessages.Items.Count == 0
                                                               ? ColumnHeaderAutoResizeStyle.HeaderSize
                                                               : ColumnHeaderAutoResizeStyle.ColumnContent);

                int index = 0;
                foreach (EveMailMessagesColumnSettings column in m_columns.Where(x => x.Visible))
                {
                    if (column.Width == -1)
                        lvMailMessages.AutoResizeColumn(index, resizeStyle);

                    index++;
                }
            }
            finally
            {
                lvMailMessages.EndUpdate();
                m_isUpdatingColumns = false;
            }
        }

        /// <summary>
        /// Updates the content of the listview.
        /// </summary>
        private void UpdateContent()
        {
            // Returns if not visible
            if (!Visible)
                return;

            // Store the selected item (if any) to restore it after the update
            int selectedItem = (lvMailMessages.SelectedItems.Count > 0
                                    ? lvMailMessages.SelectedItems[0].Tag.GetHashCode()
                                    : 0);

            lvMailMessages.BeginUpdate();
            splitContainerMailMessages.Visible = false;
            try
            {
                string text = m_textFilter.ToLowerInvariant();
                IEnumerable<EveMailMessage> eveMailMessages = m_list.Where(x => IsTextMatching(x, text));

                UpdateSort();

                switch (m_grouping)
                {
                    case EVEMailMessagesGrouping.State:
                        IOrderedEnumerable<IGrouping<EVEMailState, EveMailMessage>> groups0 =
                            eveMailMessages.GroupBy(x => x.State).OrderBy(x => (int)x.Key);
                        UpdateContent(groups0);
                        break;
                    case EVEMailMessagesGrouping.StateDesc:
                        IOrderedEnumerable<IGrouping<EVEMailState, EveMailMessage>> groups1 =
                            eveMailMessages.GroupBy(x => x.State).OrderByDescending(x => (int)x.Key);
                        UpdateContent(groups1);
                        break;
                    case EVEMailMessagesGrouping.SentDate:
                        IOrderedEnumerable<IGrouping<DateTime, EveMailMessage>> groups2 =
                            eveMailMessages.GroupBy(x => x.SentDate.Date).OrderBy(x => x.Key);
                        UpdateContent(groups2);
                        break;
                    case EVEMailMessagesGrouping.SentDateDesc:
                        IOrderedEnumerable<IGrouping<DateTime, EveMailMessage>> groups3 =
                            eveMailMessages.GroupBy(x => x.SentDate.Date).OrderByDescending(x => x.Key);
                        UpdateContent(groups3);
                        break;
                    case EVEMailMessagesGrouping.Sender:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups4 =
                            eveMailMessages.GroupBy(x => x.Sender).OrderBy(x => x.Key);
                        UpdateContent(groups4);
                        break;
                    case EVEMailMessagesGrouping.SenderDesc:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups5 =
                            eveMailMessages.GroupBy(x => x.Sender).OrderByDescending(x => x.Key);
                        UpdateContent(groups5);
                        break;
                    case EVEMailMessagesGrouping.Subject:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups6 =
                            eveMailMessages.GroupBy(x => x.Title).OrderBy(x => x.Key);
                        UpdateContent(groups6);
                        break;
                    case EVEMailMessagesGrouping.SubjectDesc:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups7 =
                            eveMailMessages.GroupBy(x => x.Title).OrderByDescending(x => x.Key);
                        UpdateContent(groups7);
                        break;
                    case EVEMailMessagesGrouping.Recipient:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups8 =
                            eveMailMessages.GroupBy(x => x.ToCharacters[0]).OrderBy(x => x.Key);
                        UpdateContent(groups8);
                        break;
                    case EVEMailMessagesGrouping.RecipientDesc:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups9 =
                            eveMailMessages.GroupBy(x => x.ToCharacters[0]).OrderByDescending(x => x.Key);
                        UpdateContent(groups9);
                        break;
                    case EVEMailMessagesGrouping.CorpOrAlliance:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups10 =
                            eveMailMessages.GroupBy(x => x.ToCorpOrAlliance).OrderBy(x => x.Key);
                        UpdateContent(groups10);
                        break;
                    case EVEMailMessagesGrouping.CorpOrAllianceDesc:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups11 =
                            eveMailMessages.GroupBy(x => x.ToCorpOrAlliance).OrderByDescending(x => x.Key);
                        UpdateContent(groups11);
                        break;
                    case EVEMailMessagesGrouping.MailingList:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups12 =
                            eveMailMessages.GroupBy(x => x.ToMailingLists[0]).OrderBy(x => x.Key);
                        UpdateContent(groups12);
                        break;
                    case EVEMailMessagesGrouping.MailingListDesc:
                        IOrderedEnumerable<IGrouping<string, EveMailMessage>> groups13 =
                            eveMailMessages.GroupBy(x => x.ToMailingLists[0]).OrderByDescending(x => x.Key);
                        UpdateContent(groups13);
                        break;
                }

                // Restore the selected item (if any)
                if (selectedItem > 0)
                {
                    foreach (ListViewItem lvItem in lvMailMessages.Items.Cast<ListViewItem>().Where(
                        lvItem => lvItem.Tag.GetHashCode() == selectedItem))
                    {
                        lvItem.Selected = true;
                    }
                }

                // Display or hide the "no EVE mail messages" label
                if (m_init)
                {
                    noEVEMailMessagesLabel.Visible = eveMailMessages.IsEmpty() ||
                                                     eveMailMessages.All(x => x.SentDate == DateTime.MinValue);
                    lvMailMessages.Visible = !eveMailMessages.IsEmpty();
                    splitContainerMailMessages.Visible = !noEVEMailMessagesLabel.Visible;
                }
            }
            finally
            {
                lvMailMessages.EndUpdate();
            }
        }

        /// <summary>
        /// Updates the content of the listview.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groups"></param>
        private void UpdateContent<TKey>(IEnumerable<IGrouping<TKey, EveMailMessage>> groups)
        {
            lvMailMessages.Items.Clear();
            lvMailMessages.Groups.Clear();

            // Add the groups
            foreach (IGrouping<TKey, EveMailMessage> group in groups)
            {
                string groupText;
                if (group.Key is EVEMailState)
                    groupText = ((EVEMailState)(Object)group.Key).GetHeader();
                else if (group.Key is DateTime)
                    groupText = ((DateTime)(Object)group.Key).ToShortDateString();
                else
                    groupText = group.Key.ToString();

                ListViewGroup listGroup = new ListViewGroup(groupText);
                lvMailMessages.Groups.Add(listGroup);

                // Add the items in every group
                foreach (EveMailMessage eveMailMessage in group)
                {
                    if (String.IsNullOrEmpty(eveMailMessage.MessageID.ToString()))
                        continue;

                    ListViewItem item = new ListViewItem(eveMailMessage.Sender, listGroup)
                                            { UseItemStyleForSubItems = false, Tag = eveMailMessage };

                    // Add enough subitems to match the number of columns
                    while (item.SubItems.Count < lvMailMessages.Columns.Count + 1)
                    {
                        item.SubItems.Add(String.Empty);
                    }

                    // Creates the subitems
                    for (int i = 0; i < lvMailMessages.Columns.Count; i++)
                    {
                        ColumnHeader header = lvMailMessages.Columns[i];
                        EveMailMessagesColumn column = (EveMailMessagesColumn)header.Tag;
                        SetColumn(eveMailMessage, item.SubItems[i], column);
                    }

                    lvMailMessages.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Updates the item sorter.
        /// </summary>
        private void UpdateSort()
        {
            lvMailMessages.ListViewItemSorter = new ListViewItemComparerByTag<EveMailMessage>(
                new EveMailMessagesComparer(m_sortCriteria, m_sortAscending));

            UpdateSortVisualFeedback();
        }

        /// <summary>
        /// Updates the sort feedback (the arrow on the header).
        /// </summary>
        private void UpdateSortVisualFeedback()
        {
            for (int i = 0; i < lvMailMessages.Columns.Count; i++)
            {
                EveMailMessagesColumn column = (EveMailMessagesColumn)lvMailMessages.Columns[i].Tag;
                if (m_sortCriteria == column)
                    lvMailMessages.Columns[i].ImageIndex = (m_sortAscending ? 0 : 1);
                else
                    lvMailMessages.Columns[i].ImageIndex = 2;
            }
        }

        /// <summary>
        /// Updates the listview sub-item.
        /// </summary>
        /// <param name="eveMailMessage"></param>
        /// <param name="item"></param>
        /// <param name="column"></param>
        private void SetColumn(EveMailMessage eveMailMessage, ListViewItem.ListViewSubItem item, EveMailMessagesColumn column)
        {
            switch (column)
            {
                case EveMailMessagesColumn.SenderName:
                    item.Text = eveMailMessage.Sender;
                    break;
                case EveMailMessagesColumn.Title:
                    item.Text = eveMailMessage.Title;
                    break;
                case EveMailMessagesColumn.SentDate:
                    item.Text = String.Format(CultureConstants.DefaultCulture,
                                              "{0:ddd} {0:G}", eveMailMessage.SentDate.ToLocalTime());
                    break;
                case EveMailMessagesColumn.ToCharacters:
                    item.Text = string.Join(", ", eveMailMessage.ToCharacters);
                    break;
                case EveMailMessagesColumn.ToCorpOrAlliance:
                    item.Text = eveMailMessage.ToCorpOrAlliance;
                    break;
                case EveMailMessagesColumn.ToMailingList:
                    item.Text = string.Join(", ", eveMailMessage.ToMailingLists);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Updates the pane position.
        /// </summary>
        private void UpdatePanePosition()
        {
            switch (PanePosition)
            {
                case ReadingPanePositioning.Off:
                    splitContainerMailMessages.Panel2Collapsed = true;
                    break;
                case ReadingPanePositioning.Bottom:
                    splitContainerMailMessages.Orientation = Orientation.Horizontal;
                    splitContainerMailMessages.Panel2Collapsed = false;
                    break;
                case ReadingPanePositioning.Right:
                    splitContainerMailMessages.Orientation = Orientation.Vertical;
                    splitContainerMailMessages.Panel2Collapsed = false;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Tries to open the selected mail in the EVEMon Mail Window.
        /// </summary>
        private void ReadMailLocal()
        {
            ListViewItem item = lvMailMessages.SelectedItems[0];
            EveMailMessage message = (EveMailMessage)item.Tag;

            // Show or bring to front if a window with the same EVE mail message already exists
            WindowsFactory<EveMessageWindow>.ShowByTag(message);
        }

        /// <summary>
        /// Tries to open the selected mail in EVE Gate.
        /// </summary>
        private void ReadMailExternal()
        {
            ListViewItem item = lvMailMessages.SelectedItems[0];
            EveMailMessage message = (EveMailMessage)item.Tag;
            Util.OpenURL(String.Format("{0}{1}", NetworkConstants.EVEGate,
                                       String.Format(NetworkConstants.EVEGateMailOpen, message.MessageID)));
        }

        /// <summary>
        /// Tries to reply the selected mail in EVE Gate.
        /// </summary>
        private void ReplyMailExternal()
        {
            ListViewItem item = lvMailMessages.SelectedItems[0];
            EveMailMessage message = (EveMailMessage)item.Tag;
            Util.OpenURL(String.Format("{0}{1}", NetworkConstants.EVEGate,
                                       String.Format(NetworkConstants.EVEGateMailReply, message.MessageID)));
        }

        /// <summary>
        /// Tries to reply the selected mail in EVE Gate (reply to all).
        /// </summary>
        private void ReplyAllMailExternal()
        {
            ListViewItem item = lvMailMessages.SelectedItems[0];
            EveMailMessage message = (EveMailMessage)item.Tag;
            Util.OpenURL(String.Format("{0}{1}", NetworkConstants.EVEGate,
                                       String.Format(NetworkConstants.EVEGateMailReplyAll, message.MessageID)));
        }

        /// <summary>
        /// Tries to forward the selected mail in EVE Gate.
        /// </summary>
        private void ForwardMailExternal()
        {
            ListViewItem item = lvMailMessages.SelectedItems[0];
            EveMailMessage message = (EveMailMessage)item.Tag;
            Util.OpenURL(String.Format("{0}{1}", NetworkConstants.EVEGate,
                                       String.Format(NetworkConstants.EVEGateMailForward, message.MessageID)));
        }

        /// <summary>
        /// Checks the given text matches the item.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if [is text matching] [the specified x]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsTextMatching(EveMailMessage x, string text)
        {
            return String.IsNullOrEmpty(text)
                   || x.Sender.ToLowerInvariant().Contains(text)
                   || x.Title.ToLowerInvariant().Contains(text)
                   || x.ToCorpOrAlliance.ToLowerInvariant().Contains(text)
                   || x.ToCharacters.Any(y => y.ToLowerInvariant().Contains(text))
                   || (x.EVEMailBody.BodyText.ToLowerInvariant().Contains(text));
        }

        /// <summary>
        /// Called when selection changed.
        /// </summary>
        private void OnSelectionChanged()
        {
            if (lvMailMessages.SelectedItems.Count == 0)
            {
                eveMailReadingPane.HidePane();
                return;
            }

            EveMailMessage selectedObject = lvMailMessages.SelectedItems[0].Tag as EveMailMessage;
            if (selectedObject == null)
            {
                eveMailReadingPane.HidePane();
                return;
            }

            // If we haven't done it yet, download the mail body
            if (selectedObject.EVEMailBody.MessageID == 0)
            {
                selectedObject.GetMailBody();
                return;
            }

            eveMailReadingPane.SelectedObject = selectedObject;
        }

        #endregion


        #region Local Event Handlers

        /// <summary>
        /// Shows the context menu only when a message is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = lvMailMessages.SelectedItems.Count == 0;
        }

        /// <summary>
        /// Handles the MouseHover event of the lvMailMessages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void lvMailMessages_MouseHover(object sender, EventArgs e)
        {
            Focus();
        }

        /// <summary>
        /// On resize, updates the controls visibility.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowEVEMailMessagesList_Resize(object sender, EventArgs e)
        {
            if (!m_init)
                return;

            UpdateContent();
        }

        /// <summary>
        /// When the selection update timer ticks, we process the changes caused by a selection change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            OnSelectionChanged();
        }

        /// <summary>
        /// When the user selects another item, we do not immediately process the change but rather delay it through a timer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.ListViewItemSelectionChangedEventArgs"/> instance containing the event data.</param>
        private void lvMailMessages_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (timer.Enabled)
                return;

            timer.Interval = 100;
            timer.Enabled = true;
            timer.Start();
        }

        /// <summary>
        /// Opens a window form to display the EVE mail body.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void lvMailMessages_DoubleClick(object sender, EventArgs e)
        {
            ReadMailLocal();
        }

        /// <summary>
        /// On column reorder we update the settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvMailMessages_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            m_columnsChanged = true;
        }

        /// <summary>
        /// When the user manually resizes a column, we make sure to update the column preferences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvMailMessages_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (m_isUpdatingColumns || m_columns.Count <= e.ColumnIndex)
                return;

            m_columns[e.ColumnIndex].Width = lvMailMessages.Columns[e.ColumnIndex].Width;
            m_columnsChanged = true;
        }

        /// <summary>
        /// When the user clicks a column header, we update the sorting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvMailMessages_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            EveMailMessagesColumn column = (EveMailMessagesColumn)lvMailMessages.Columns[e.Column].Tag;
            if (m_sortCriteria == column)
                m_sortAscending = !m_sortAscending;
            else
            {
                m_sortCriteria = column;
                m_sortAscending = true;
            }

            UpdateContent();
        }

        /// <summary>
        /// Picking "Open mail" in the context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mailReadLocal_Click(object sender, EventArgs e)
        {
            ReadMailLocal();
        }

        /// <summary>
        /// Picking "Open" in the EVE Gate context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mailGateRead_Click(object sender, EventArgs e)
        {
            ReadMailExternal();
        }

        /// <summary>
        /// Picking "Reply" in the EVE Gate context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mailGateReply_Click(object sender, EventArgs e)
        {
            ReplyMailExternal();
        }

        /// <summary>
        /// Picking "Reply all" in the EVE Gate context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mailGateReplyAll_Click(object sender, EventArgs e)
        {
            ReplyAllMailExternal();
        }

        /// <summary>
        /// Picking "Forward" in the EVE Gate context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mailGateForward_Click(object sender, EventArgs e)
        {
            ForwardMailExternal();
        }

        # endregion


        #region Global Events

        /// <summary>
        /// On timer tick, we update the column settings if any changes have been made to them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            if (m_columnsChanged)
            {
                Settings.UI.MainWindow.EVEMailMessages.Columns = Columns.Select(x => x.Clone()).ToArray();

                // Recreate the columns
                Columns = Settings.UI.MainWindow.EVEMailMessages.Columns;
            }

            m_columnsChanged = false;
        }

        /// <summary>
        /// When the mail messages change update the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterEVEMailMessagesUpdated(object sender, CharacterChangedEventArgs e)
        {
            CCPCharacter ccpCharacter = Character as CCPCharacter;
            if (ccpCharacter == null || e.Character != ccpCharacter)
                return;

            EVEMailMessages = ccpCharacter.EVEMailMessages;
            UpdateColumns();
        }

        /// <summary>
        /// When the mail message body gets downloaded update the reading pane.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterEVEMailBodyDownloaded(object sender, CharacterChangedEventArgs e)
        {
            CCPCharacter ccpCharacter = Character as CCPCharacter;
            if (e.Character != ccpCharacter)
                return;

            OnSelectionChanged();
        }

        /// <summary>
        /// Handles the NotificationSent event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EVEMon.Common.Notifications.NotificationEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_NotificationSent(object sender, NotificationEventArgs e)
        {
            if (!(e is APIErrorNotificationEventArgs))
                return;

            if (!(((APIErrorNotificationEventArgs)e).Result is APIResult<SerializableAPIMailBodies>))
                return;

            // In case there was an error, hide the pane
            if (((APIErrorNotificationEventArgs)e).Result.HasError)
                eveMailReadingPane.HidePane();
        }

        # endregion
    }
}