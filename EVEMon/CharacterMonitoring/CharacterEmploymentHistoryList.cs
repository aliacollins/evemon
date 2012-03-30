using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon.CharacterMonitoring
{
    public partial class CharacterEmploymentHistoryList : UserControl
    {
        private CCPCharacter m_ccpCharacter;

        private const int PadTop = 2;
        private const int PadLeft = 6;
        private const TextFormatFlags TextFormat = TextFormatFlags.NoPadding | TextFormatFlags.NoClipping;
        private const string DateTimeFormat = "{0:yyyy.MM.dd} {0:HH:mm}";
        private const string RecordDateFromText = "From";
        private const string RecordDateToText = "to";
        private const string RecordDateTodayText = "this day";

        // Employment History drawing - Employment record
        private const int EmploymentRecordDetailHeight = 34;

        private readonly Font m_recordFont;
        private readonly Font m_recordBoldFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterEmploymentHistoryList"/> class.
        /// </summary>
        public CharacterEmploymentHistoryList()
        {
            InitializeComponent();

            lbEmploymentHistory.Visible = false;

            m_recordFont = FontFactory.GetFont("Tahoma", 8.25F);
            m_recordBoldFont = FontFactory.GetFont("Tahoma", 8.25F, FontStyle.Bold);
            noEmploymentHistoryLabel.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);

            EveMonClient.CharacterInfoUpdated += EveMonClient_CharacterInfoUpdated;
            EveMonClient.EveIDToNameUpdated += EveMonClient_EveIDToNameUpdated;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// Gets the character associated with this monitor.
        /// </summary>
        [Browsable(false)]
        public Character Character { get; set; }


        /// <summary>
        /// Gets the item's height.
        /// </summary>
        private int GetItemHeight
        {
            get { return Math.Max(m_recordFont.Height * 2 + PadTop * 2, EmploymentRecordDetailHeight); }
        }


        #region Inherited events

        /// <summary>
        /// Unsubscribe events on disposing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.CharacterInfoUpdated -= EveMonClient_CharacterInfoUpdated;
            EveMonClient.EveIDToNameUpdated -= EveMonClient_EveIDToNameUpdated;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// When the control becomes visible again, we update the content.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
                UpdateContent();
        }

        #endregion


        #region Content Management

        /// <summary>
        /// Updates all the content.
        /// </summary>
        private void UpdateContent()
        {
            // Returns if not visible
            if (!Visible)
                return;

            // When no character, we just hide the list
            if (Character == null)
            {
                noEmploymentHistoryLabel.Visible = true;
                lbEmploymentHistory.Visible = false;
                return;
            }

            m_ccpCharacter = Character as CCPCharacter;

            // If the character is not a CCPCharacter it does not have employment history
            if (m_ccpCharacter == null)
                return;

            int scrollBarPosition = lbEmploymentHistory.TopIndex;

            // Update the skills list
            lbEmploymentHistory.BeginUpdate();
            try
            {
                // Add items in the list
                lbEmploymentHistory.Items.Clear();
                foreach (EmploymentRecord employmentRecord in m_ccpCharacter.EmploymentHistory)
                {
                    employmentRecord.EmploymentRecordImageUpdated += record_EmploymentRecordImageUpdated;
                    lbEmploymentHistory.Items.Add(employmentRecord);
                }

                // Display or hide the "no skills" label.
                noEmploymentHistoryLabel.Visible = !m_ccpCharacter.EmploymentHistory.Any();
                lbEmploymentHistory.Visible = m_ccpCharacter.EmploymentHistory.Any();

                // Invalidate display
                lbEmploymentHistory.Invalidate();
            }
            finally
            {
                lbEmploymentHistory.EndUpdate();
                lbEmploymentHistory.TopIndex = scrollBarPosition;
            }
        }

        #endregion


        #region Drawing

        /// <summary>
        /// Handles the MeasureItem event of the lbEmploymentHistory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MeasureItemEventArgs"/> instance containing the event data.</param>
        private void lbEmploymentHistory_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.ItemHeight = GetItemHeight;
        }

        /// <summary>
        /// Draws the list item for the given standing
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.DrawItemEventArgs"/> instance containing the event data.</param>
        private void lbEmploymentHistory_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lbEmploymentHistory.Items.Count)
                return;

            Graphics g = e.Graphics;

            EmploymentRecord record = (EmploymentRecord)lbEmploymentHistory.Items[e.Index];
            EmploymentRecord previousRecord = e.Index == 0 ? null : (EmploymentRecord)lbEmploymentHistory.Items[e.Index - 1];

            // Draw background
            g.FillRectangle((e.Index % 2) == 0 ? Brushes.White : Brushes.LightGray, e.Bounds);

            // Measure texts
            DateTime dt = previousRecord == null ? DateTime.UtcNow : previousRecord.StartDate;
            string recordPeriodText = String.Format(CultureConstants.DefaultCulture, "( {0} )",
                                                    dt.Subtract(record.StartDate).ToDescriptiveText(
                                                        DescriptiveTextOptions.SpaceBetween, false).Trim());
            string recordStartDateText = String.Format(CultureConstants.DefaultCulture, DateTimeFormat,
                                                       record.StartDate.ToLocalTime());
            string recordEndDateText = previousRecord == null
                                           ? RecordDateTodayText
                                           : String.Format(CultureConstants.DefaultCulture, DateTimeFormat,
                                                           previousRecord.StartDate.ToLocalTime());

            Size recordCorporationNameTextSize = TextRenderer.MeasureText(g, record.CorporationName, m_recordBoldFont, Size.Empty,
                                                                          TextFormat);
            Size recordPeriodTextSize = TextRenderer.MeasureText(g, recordPeriodText, m_recordFont, Size.Empty, TextFormat);
            Size recordFromTextSize = TextRenderer.MeasureText(g, RecordDateFromText, m_recordFont, Size.Empty, TextFormat);
            Size recordStartDateTextSize = TextRenderer.MeasureText(g, recordStartDateText, m_recordBoldFont, Size.Empty,
                                                                    TextFormat);
            Size recordToTextSize = TextRenderer.MeasureText(g, RecordDateToText, m_recordFont, Size.Empty, TextFormat);
            Size recordEndDateTextSize = TextRenderer.MeasureText(g, recordEndDateText,
                                                                  previousRecord == null ? m_recordFont : m_recordBoldFont,
                                                                  Size.Empty, TextFormat);

            // Draw texts
            TextRenderer.DrawText(g, record.CorporationName, m_recordBoldFont,
                                  new Rectangle(
                                      e.Bounds.Left + PadLeft * 7,
                                      e.Bounds.Top + PadTop,
                                      recordCorporationNameTextSize.Width + PadLeft,
                                      recordCorporationNameTextSize.Height), Color.Black);

            TextRenderer.DrawText(g, RecordDateFromText, m_recordFont,
                                  new Rectangle(
                                      e.Bounds.Left + PadLeft * 7,
                                      e.Bounds.Top + PadTop + recordCorporationNameTextSize.Height,
                                      recordFromTextSize.Width + PadLeft,
                                      recordFromTextSize.Height), Color.Black);

            TextRenderer.DrawText(g, recordStartDateText, m_recordBoldFont,
                                  new Rectangle(
                                      e.Bounds.Left + PadLeft * 7 + recordFromTextSize.Width + PadLeft,
                                      e.Bounds.Top + PadTop + recordCorporationNameTextSize.Height,
                                      recordStartDateTextSize.Width + PadLeft,
                                      recordStartDateTextSize.Height), Color.Black);

            TextRenderer.DrawText(g, RecordDateToText, m_recordFont,
                                  new Rectangle(
                                      e.Bounds.Left + PadLeft * 7 + recordFromTextSize.Width + recordStartDateTextSize.Width +
                                      PadLeft * 2,
                                      e.Bounds.Top + PadTop + recordCorporationNameTextSize.Height,
                                      recordToTextSize.Width + PadLeft,
                                      recordToTextSize.Height), Color.Black);

            TextRenderer.DrawText(g, recordEndDateText, previousRecord == null ? m_recordFont : m_recordBoldFont,
                                  new Rectangle(
                                      e.Bounds.Left + PadLeft * 7 + recordFromTextSize.Width + recordStartDateTextSize.Width +
                                      recordToTextSize.Width + PadLeft * 3,
                                      e.Bounds.Top + PadTop + recordCorporationNameTextSize.Height,
                                      recordEndDateTextSize.Width + PadLeft,
                                      recordEndDateTextSize.Height), Color.Black);

            TextRenderer.DrawText(g, recordPeriodText, m_recordFont,
                                  new Rectangle(
                                      e.Bounds.Left + PadLeft * 7 + recordFromTextSize.Width + recordStartDateTextSize.Width +
                                      recordToTextSize.Width + recordEndDateTextSize.Width + PadLeft * 4,
                                      e.Bounds.Top + PadTop + recordCorporationNameTextSize.Height,
                                      recordPeriodTextSize.Width + PadLeft,
                                      recordPeriodTextSize.Height), Color.Black);

            // Draw the corporation image
            if (Settings.UI.SafeForWork)
                return;

            g.DrawImage(record.CorporationImage,
                        new Rectangle(e.Bounds.Left + PadLeft / 2,
                                      (EmploymentRecordDetailHeight / 2) - (record.CorporationImage.Height / 2) + e.Bounds.Top,
                                      record.CorporationImage.Width, record.CorporationImage.Height));
        }

        /// <summary>
        /// Gets the preferred size from the preferred size of the skills list.
        /// </summary>
        /// <param name="proposedSize"></param>
        /// <returns></returns>
        public override Size GetPreferredSize(Size proposedSize)
        {
            return lbEmploymentHistory.GetPreferredSize(proposedSize);
        }

        #endregion


        #region Local events

        /// <summary>
        /// When the image updates, we redraw the list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void record_EmploymentRecordImageUpdated(object sender, EventArgs e)
        {
            // Force to redraw
            lbEmploymentHistory.Invalidate();
        }

        /// <summary>
        /// Handles the MouseWheel event of the lbEmploymentHistory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void lbEmploymentHistory_MouseWheel(object sender, MouseEventArgs e)
        {
            // Update the drawing based upon the mouse wheel scrolling
            int numberOfItemLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            int lines = numberOfItemLinesToMove;
            if (lines == 0)
                return;

            // Compute the number of lines to move
            int direction = lines / Math.Abs(lines);
            int[] numberOfPixelsToMove = new int[lines * direction];
            for (int i = 1; i <= Math.Abs(lines); i++)
            {
                object item = null;

                // Going up
                if (direction == Math.Abs(direction))
                {
                    // Retrieve the next top item
                    if (lbEmploymentHistory.TopIndex - i >= 0)
                        item = lbEmploymentHistory.Items[lbEmploymentHistory.TopIndex - i];
                }
                    // Going down
                else
                {
                    // Compute the height of the items from current the topindex (included)
                    int height = 0;
                    for (int j = lbEmploymentHistory.TopIndex + i - 1; j < lbEmploymentHistory.Items.Count; j++)
                    {
                        height += GetItemHeight;
                    }

                    // Retrieve the next bottom item
                    if (height > lbEmploymentHistory.ClientSize.Height)
                        item = lbEmploymentHistory.Items[lbEmploymentHistory.TopIndex + i - 1];
                }

                // If found a new item as top or bottom
                if (item != null)
                    numberOfPixelsToMove[i - 1] = GetItemHeight * direction;
                else
                    lines -= direction;
            }

            // Scroll 
            if (lines != 0)
                lbEmploymentHistory.Invalidate();
        }

        #endregion


        #region Global events

        /// <summary>
        /// When the character standings update, we update the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_CharacterInfoUpdated(object sender, CharacterChangedEventArgs e)
        {
            if (e.Character != Character)
                return;

            UpdateContent();
        }

        /// <summary>
        /// When the settings change, we update the content.
        /// </summary>
        /// <remarks>In case 'SafeForWork' gets enabled.</remarks>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// When the EveIDToName updates, we refresh the content.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_EveIDToNameUpdated(object sender, EventArgs e)
        {
            // Force to redraw
            lbEmploymentHistory.Invalidate();
        }

        #endregion
    }
}
