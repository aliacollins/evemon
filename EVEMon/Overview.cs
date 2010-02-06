using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.SettingsObjects;

namespace EVEMon
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Overview : UserControl
    {
        public event EventHandler<CharacterChangedEventArgs> CharacterClicked;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Overview()
        {
            InitializeComponent();
            this.AutoScroll = true;

            EveClient.SettingsChanged += new EventHandler(EveClient_SettingsChanged);
            EveClient.MonitoredCharacterCollectionChanged += new EventHandler(EveClient_MonitoredCharacterCollectionChanged);
            this.Disposed += new EventHandler(Overview_Disposed);

        }

        /// <summary>
        /// On disposing, unsubscribe events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Overview_Disposed(object sender, EventArgs e)
        {
            EveClient.SettingsChanged -= new EventHandler(EveClient_SettingsChanged);
            EveClient.MonitoredCharacterCollectionChanged -= new EventHandler(EveClient_MonitoredCharacterCollectionChanged);
            this.Disposed -= new EventHandler(Overview_Disposed);
        }

        /// <summary>
        /// On load, update the controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.DesignMode || this.IsDesignModeHosted()) return;

            UpdateContent();
        }

        #region Content creation and layout
        /// <summary>
        /// Updates the characters' list with the provided monitors
        /// </summary>
        /// <param name="monitors"></param>
        public void UpdateContent()
        {
            this.SuspendLayout();
            try
            {
                CleanUp();

                if (EveClient.MonitoredCharacters.IsEmpty())
                {
                    // Updates the visibility of the label for when no characters are loaded
                    labelNoCharacters.Visible = true;
                    return;
                }

                // Creates the controls
                labelNoCharacters.Visible = false;
                var characters = new List<Character>();
                if(Settings.UI.MainWindow.PutTrainingSkillsFirstOnOverview)
                {
                    characters.AddRange(EveClient.MonitoredCharacters.Where(x => x.IsTraining));
                    characters.AddRange(EveClient.MonitoredCharacters.Where(x => !x.IsTraining));
                }
                else
                {
                    characters.AddRange(EveClient.MonitoredCharacters);
                }
                foreach (var character in characters)
                {
                    // Creates a control and adds it
                    var item = new OverviewItem(character, Settings.UI.MainWindow);
                    item.Click += new EventHandler(item_Click);
                    item.Clickable = true;

                    // Add it 
                    this.Controls.Add(item);
                }

                PerformCustomLayout();
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// Cleans up the existing controls
        /// </summary>
        internal void CleanUp()
        {
            // Backup the controls list
            Control[] oldControls = new Control[this.Controls.Count];
            this.Controls.CopyTo(oldControls, 0);

            // Clear the controls list
            this.Controls.Clear();
            this.Controls.Add(labelNoCharacters);

            // Dispose every one of the control to prevent timer's execution
            foreach (Control item in oldControls)
            {
                if (item == labelNoCharacters) continue;
                item.Click -= new EventHandler(item_Click);
                item.Dispose();
            }
        }

        /// <summary>
        /// Updates the number of rows and columns
        /// </summary>
        /// <remarks>
        /// Cannot use a tableLayoutPanel in the end : too slow, too buggy.
        /// </remarks>
        private void PerformCustomLayout()
        {
            const int padding = 20;
            this.SuspendLayout();
            try
            {
                // Check there is at least one control
                int numControls = this.Controls.Count - 1;
                if (numControls <= 0) return;

                // Retrieve the item width (should be the same for all controls) and compute the item and row width
                var firstItem = (OverviewItem)this.Controls[1];
                int itemWidth = firstItem.PreferredSize.Width;

                // Computes the number of columns and rows we need
                int numColumns = Math.Max(1, Math.Min(numControls, this.ClientSize.Width / itemWidth));
                int numRows = (this.Controls.Count + numColumns - 1) / numColumns;

                // Computes the horizontal margin
                int neededWidth = numColumns * (itemWidth + padding) - padding;
                int marginH = Math.Max(0, (this.ClientSize.Width - neededWidth) >> 1);

                // Measure the total height
                int index = 0;
                int rowIndex = 0;
                int rowHeight = 0;
                int height = 0;
                foreach (Control ctl in this.Controls)
                {
                    // Skip the "no characters" label
                    if (ctl == labelNoCharacters) continue;

                    // Add the item to the row
                    rowHeight = Math.Max(rowHeight, ctl.PreferredSize.Height);
                    rowIndex++;
                    index++;

                    // Skip if row not complete yet
                    if (rowIndex != numColumns && index != this.Controls.Count) continue;
                    height += rowHeight + padding;
                    rowHeight = 0;
                    rowIndex = 0;
                }

                // Computes the vertical margin
                height -= padding;
                int marginV = Math.Max(0, (this.ClientSize.Height - height) / 3); // We puts 1/3 at the top, 2/3 at the bottom


                // Adjust the controls bounds
                rowIndex = 0;
                rowHeight = 0;
                height = marginV;
                foreach (Control ctl in this.Controls)
                {
                    // Skip the "no characters" label
                    if (ctl == labelNoCharacters) continue;

                    // Set the control bound
                    ctl.SetBounds(marginH + rowIndex * (itemWidth + padding), height, ctl.PreferredSize.Width, ctl.PreferredSize.Height);
                    rowHeight = Math.Max(rowHeight, ctl.PreferredSize.Height);
                    rowIndex++;

                    // Skip if row not complete yet
                    if (rowIndex != numColumns && index != this.Controls.Count) continue;
                    height += rowHeight + padding;
                    rowHeight = 0;
                    rowIndex = 0;
                }
            }
            finally
            {
                this.ResumeLayout(true);
            }
        }
        #endregion


        #region Globals and locals events
        /// <summary>
        /// When the settings changed, we need to rebuild the items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// Occur when the monitored characters collection changed. We update the layout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EveClient_MonitoredCharacterCollectionChanged(object sender, EventArgs e)
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
        void item_Click(object sender, EventArgs e)
        {
            var item = sender as OverviewItem;
            if (this.CharacterClicked != null)
            {
                this.CharacterClicked(this, new CharacterChangedEventArgs(item.Character));
            }
        }
        #endregion
    }
}
