using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.CustomEventArgs;

namespace EVEMon.Controls
{
    public partial class Overview : UserControl
    {
        public event EventHandler<CharacterChangedEventArgs> CharacterClicked;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Overview()
        {
            InitializeComponent();
        }

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

            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            EveMonClient.MonitoredCharacterCollectionChanged += EveMonClient_MonitoredCharacterCollectionChanged;
            Disposed += OnDisposed;

            UpdateContent();
        }

        /// <summary>
        /// On disposing, unsubscribe events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDisposed(object sender, EventArgs e)
        {
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            EveMonClient.MonitoredCharacterCollectionChanged -= EveMonClient_MonitoredCharacterCollectionChanged;
            Disposed -= OnDisposed;
        }


        #region Content creation and layout

        /// <summary>
        /// Updates the characters' list with the provided monitors.
        /// </summary>
        private void UpdateContent()
        {
            SuspendLayout();
            try
            {
                CleanUp();

                // Updates the visibility of the label for when no characters are loaded
                if (!EveMonClient.MonitoredCharacters.Any())
                {
                    labelNoCharacters.Visible = true;
                    return;
                }

                // Creates the controls
                List<Character> characters = new List<Character>();
                if (Settings.UI.MainWindow.PutTrainingSkillsFirstOnOverview)
                {
                    characters.AddRange(EveMonClient.MonitoredCharacters.Where(x => x.IsTraining));
                    characters.AddRange(EveMonClient.MonitoredCharacters.Where(x => !x.IsTraining));
                }
                else
                    characters.AddRange(EveMonClient.MonitoredCharacters);

                foreach (OverviewItem item in characters.Select(character => new OverviewItem(character, Settings.UI.MainWindow)))
                {
                    item.Click += item_Click;
                    item.Clickable = true;

                    // Ensure that the control gets created before we add it,
                    // (when Overview is created and then we hide a character,
                    // the control gets created after the custom layout has been performed,
                    // causing the controls to get misplaced)
                    item.CreateControl();

                    // Add it 
                    Controls.Add(item);
                }

                PerformCustomLayout();
            }
            finally
            {
                ResumeLayout();
            }
        }

        /// <summary>
        /// Cleans up the existing controls.
        /// </summary>
        private void CleanUp()
        {
            // Dispose every one of the control to prevent event triggering
            foreach (OverviewItem item in Controls.OfType<OverviewItem>())
            {
                item.Click -= item_Click;
                item.Dispose();
            }

            // Clear the controls list
            Controls.Clear();
            Controls.Add(labelNoCharacters);
        }

        /// <summary>
        /// Updates the number of rows and columns.
        /// </summary>
        /// <remarks>
        /// Cannot use a tableLayoutPanel in the end : too slow, too buggy.
        /// </remarks>
        private void PerformCustomLayout()
        {
            // Check there is at least one control
            int numControls = Controls.OfType<OverviewItem>().Count();
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
                OverviewItem firstItem = Controls.OfType<OverviewItem>().First();
                int itemWidth = firstItem.PreferredSize.Width;

                // Computes the number of columns and rows we need
                int numColumns = Math.Max(1, Math.Min(numControls, ClientSize.Width / itemWidth));

                // Computes the horizontal margin
                int neededWidth = numColumns * (itemWidth + Pad) - Pad;
                int marginH = Math.Max(0, (ClientSize.Width - neededWidth) / 2);

                // Measure the total height
                int rowIndex = 0;
                int rowHeight = 0;
                int height = 0;
                foreach (OverviewItem control in Controls.OfType<OverviewItem>())
                {
                    // Add the item to the row
                    rowHeight = Math.Max(rowHeight, control.PreferredSize.Height);
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
                foreach (OverviewItem control in Controls.OfType<OverviewItem>())
                {
                    // Set the control bound
                    control.SetBounds(marginH + rowIndex * (itemWidth + Pad), height, control.PreferredSize.Width,
                                      control.PreferredSize.Height);
                    rowHeight = Math.Max(rowHeight, control.PreferredSize.Height);
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

                // Restore the scroll bar position
                VerticalScroll.Value = scrollBarPosition;
            }
        }

        #endregion


        #region Globals and locals events

        /// <summary>
        /// When the settings changed, we need to rebuild the items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

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
        /// Adjust the layout on size change.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            PerformCustomLayout();
            base.OnSizeChanged(e);
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