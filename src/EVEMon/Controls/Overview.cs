using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Factories;
using EVEMon.Common.Models;

namespace EVEMon.Controls
{
    public partial class Overview : UserControl
    {
        public event EventHandler<CharacterChangedEventArgs> CharacterClicked;

        private bool m_grouping;


        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Overview()
        {
            InitializeComponent();

            labelNoCharacters.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
            labelLoading.Font = FontFactory.GetFont("Tahoma", 11.25F, FontStyle.Bold);
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// On load, update the controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode || this.IsDesignModeHosted())
                return;

            DoubleBuffered = true;

            EveMonClient.MonitoredCharacterCollectionChanged += EveMonClient_MonitoredCharacterCollectionChanged;
            EveMonClient.CharacterUpdated += EveMonClient_CharacterUpdated;
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            Disposed += OnDisposed;
        }

        /// <summary>
        /// On visibility, we may need to refresh the display.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (DesignMode || this.IsDesignModeHosted())
                return;

            if (Visible)
                UpdateContent();
        }

        /// <summary>
        /// On disposing, unsubscribe events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.MonitoredCharacterCollectionChanged -= EveMonClient_MonitoredCharacterCollectionChanged;
            EveMonClient.CharacterUpdated -= EveMonClient_CharacterUpdated;
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Disposed -= OnDisposed;
        }

        /// <summary>
        /// Adjust the layout on size change.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            PerformCustomLayout();
            base.OnSizeChanged(e);
        }

        #endregion


        #region Content creation and layout

        /// <summary>
        /// Updates the overview content.
        /// </summary>
        private void UpdateContent()
        {
            // Updates the visibility of the label for when no characters are loaded
            if (!EveMonClient.MonitoredCharacters.Any())
            {
                if (Controls.OfType<OverviewItem>().Any())
                    CleanUp(Controls.OfType<OverviewItem>());

                labelNoCharacters.Show();
                return;
            }

            // Collect the existing overview items
            Dictionary<Character, OverviewItem> items = Controls.OfType<OverviewItem>().ToDictionary(page => (Character)page.Tag);

            // Create the order we will layout the controls
            List<Character> characters = new List<Character>();
            m_grouping = Settings.UI.MainWindow.PutTrainingSkillsFirstOnOverview;
            if (m_grouping)
            {
                characters.AddRange(EveMonClient.MonitoredCharacters.Where(x => x.IsTraining));
                characters.AddRange(EveMonClient.MonitoredCharacters.Where(x => !x.IsTraining));
            }
            else
                characters.AddRange(EveMonClient.MonitoredCharacters);

            int index = 0;
            List<OverviewItem> overviewItems = Controls.OfType<OverviewItem>().ToList();
            foreach (Character character in characters)
            {
                // Retrieve the current overview item, or null if we're past the limits
                OverviewItem currentOverviewItem = (index < overviewItems.Count ? overviewItems[index] : null);

                // Does the overview item match with the character ?
                if ((Character)currentOverviewItem?.Tag != character)
                {
                    // Retrieve the overview item when it was previously created
                    // Is the overview item later in the collection ?
                    OverviewItem overviewItem;
                    if (items.TryGetValue(character, out overviewItem))
                        overviewItems.Remove(overviewItem); // Remove the overview item from old location
                    else
                        overviewItem = GetOverviewItem(character); // Create a new overview item

                    // Inserts the overview item in the proper location
                    overviewItems.Insert(index, overviewItem);
                }

                // Remove processed character from the dictionary and move forward
                if (character != null)
                    items.Remove(character);

                index++;
            }

            // Remove the remaining items
            CleanUp(items.Values);
            foreach (OverviewItem item in items.Values)
            {
                overviewItems.Remove(item);
            }

            // Add the created items to the Overview
            Controls.AddRange(overviewItems.ToArray<Control>());

            PerformCustomLayout();
        }

        /// <summary>
        /// Cleans up the existing controls.
        /// </summary>
        private void CleanUp(IEnumerable<OverviewItem> items)
        {
            // Dispose every one of the control to prevent event triggering
            foreach (OverviewItem item in items)
            {
                item.Click -= item_Click;
                item.Dispose();
            }

            // Clear the controls list
            Controls.Clear();
            Controls.Add(labelNoCharacters);
        }

        /// <summary>
        /// Gets the overview item.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        private OverviewItem GetOverviewItem(Character character)
        {
            OverviewItem overviewItem;
            OverviewItem tempOverviewItem = null;
            try
            {
                // Creates a new page
                tempOverviewItem = new OverviewItem(character);
                tempOverviewItem.Click += item_Click;
                tempOverviewItem.Clickable = true;
                tempOverviewItem.Tag = character;

                // Ensure that the control gets created before we add it,
                // (when Overview is created and then we hide a character,
                // the control gets created after the custom layout has been performed,
                // causing the controls to get misplaced)
                tempOverviewItem.CreateControl();

                overviewItem = tempOverviewItem;
                tempOverviewItem = null;
            }
            finally
            {
                tempOverviewItem?.Dispose();
            }

            return overviewItem;
        }

        /// <summary>
        /// Updates the number of rows and columns.
        /// </summary>
        /// <remarks>
        /// Cannot use a tableLayoutPanel in the end : too slow, too buggy.
        /// </remarks>
        private void PerformCustomLayout()
        {
            if (!Visible)
                return;

            IList<OverviewItem> overviewItems = Controls.OfType<OverviewItem>().ToList();

            // Check there is at least one control
            int numControls = overviewItems.Count;
            if (numControls == 0)
                return;

            const int Pad = 20;

            // Store and reset the scroll bar position
            int scrollBarPosition = VerticalScroll.Value;
            VerticalScroll.Value = 0;

            SuspendLayout();
            try
            {
                // Retrieve the item width (should be the same for all controls) and compute the item and row width
                int itemWidth = overviewItems.First().PreferredSize.Width;

                // Computes the number of columns and rows we need
                int numColumns = Math.Max(1, Math.Min(numControls, ClientSize.Width / itemWidth));

                // Computes the horizontal margin
                int neededWidth = numColumns * (itemWidth + Pad) - Pad;
                int marginH = Math.Max(0, (ClientSize.Width - neededWidth) / 2);

                // Measure the total height
                int rowIndex = 0;
                int rowHeight = 0;
                int height = 0;
                foreach (OverviewItem overviewItem in overviewItems)
                {
                    // Add the item to the row
                    rowHeight = Math.Max(rowHeight, overviewItem.PreferredSize.Height);
                    rowIndex++;

                    // Skip if row not complete yet
                    if (rowIndex != numColumns)
                        continue;

                    height += rowHeight + Pad;
                    rowHeight = 0;
                    rowIndex = 0;
                }

                // Computes the vertical margin
                height -= Pad;
                int marginV = Math.Max(0, (ClientSize.Height - height) / 3); // We put 1/3 at the top, 2/3 at the bottom

                // Adjust the controls bounds
                rowIndex = 0;
                rowHeight = 0;
                height = marginV;
                foreach (OverviewItem overviewItem in overviewItems)
                {
                    // Set the control bound
                    overviewItem.SetBounds(marginH + rowIndex * (itemWidth + Pad), height, overviewItem.PreferredSize.Width,
                                           overviewItem.PreferredSize.Height);
                    rowHeight = Math.Max(rowHeight, overviewItem.PreferredSize.Height);
                    rowIndex++;

                    // Skip if row not complete yet
                    if (rowIndex != numColumns)
                        continue;

                    height += rowHeight + Pad;
                    rowHeight = 0;
                    rowIndex = 0;
                }
            }
            finally
            {
                ResumeLayout(true);
                labelNoCharacters.Visible = !EveMonClient.MonitoredCharacters.Any();

                // Restore the scroll bar position
                VerticalScroll.Value = scrollBarPosition;
            }
        }

        #endregion


        #region Globals and locals events

        /// <summary>
        /// Occur when the monitored characters collection changed. We update the layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_MonitoredCharacterCollectionChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// When aby character updates, we update the layout.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EVEMon.Common.CustomEventArgs.CharacterChangedEventArgs"/> instance containing the event data.</param>
        private void EveMonClient_CharacterUpdated(object sender, CharacterChangedEventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// When the settings changed, update if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            labelLoading.Hide();

            // Update only when grouping settings have changed
            if (m_grouping != Settings.UI.MainWindow.PutTrainingSkillsFirstOnOverview)
                UpdateContent();
        }

        /// <summary>
        /// When an item has been clicked, fires the appropriate event to notify the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void item_Click(object sender, EventArgs e)
        {
            OverviewItem item = sender as OverviewItem;
            if (CharacterClicked != null && item != null)
                CharacterClicked(this, new CharacterChangedEventArgs(item.Character));
        }

        #endregion
    }
}