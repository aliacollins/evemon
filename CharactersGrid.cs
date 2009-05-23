using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CharactersGrid : UserControl, IMainWindowPage
    {
        #region CharacterClickedEventArgs
        /// <summary>
        /// 
        /// </summary>
        public sealed class CharacterClickedEventArgs : EventArgs
        {
            public CharacterMonitor CharacterMonitor
            {
                get;
                set;
            }
        }
        #endregion

        public delegate void CharacterClickedHandler(object sender, CharacterClickedEventArgs e);
        public event CharacterClickedHandler CharacterClicked;

        private const int WidthPerCharacter = 300;

        public CharactersGrid()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            this.Load += new EventHandler(CharactersGrid_Load);
        }

        private Settings m_settings;
        private bool m_currentlyVisible;

        void CharactersGrid_Load(object sender, EventArgs e)
        {
            m_settings = Settings.GetInstance();
        }

        /// <summary>
        /// Gets or sets true when the control is currently the visible tab. This is used to prevent unnecessary UI updates
        /// </summary>
        public bool CurrentlyVisible
        {
            get { return m_currentlyVisible; }
            set
            {
                if (m_currentlyVisible != value)
                {
                    m_currentlyVisible = value;

                    foreach (CharactersGridItem item in tableLayoutPanel.Controls)
                    {
                        if (value) item.Start();
                        else item.Stop();
                    }

                    if (value) timer.Start();
                    else timer.Stop();
                }
            }
        }

        /// <summary>
        /// Updates the characters' list with the provided monitors
        /// </summary>
        /// <param name="monitors"></param>
        public void UpdateCharactersList(IEnumerable<CharacterMonitor> monitors)
        {
            CleanUp();

            // Creates the controls and adjust layout
            if (monitors != null)
            {
                // Creates the controls
                foreach (var monitor in monitors)
                {
                    // Creates a control and adds it
                    var item = new CharactersGridItem(monitor);
                    this.tableLayoutPanel.Controls.Add(item);

                    // Adjusts visibility
                    if (m_currentlyVisible) item.Start();
                    else item.Stop();

                    // Subscribe events
                    item.Click += new EventHandler(item_Click);
                }

                // Adjusts the layout
                UpdateLayout();
            }

            // Updates the visibility of the label for when no characters are loaded
            labelNoCharacters.Visible = (tableLayoutPanel.Controls.Count == 0);
        }

        /// <summary>
        /// Cleans up the existing controls
        /// </summary>
        internal void CleanUp()
        {
            // Backup the controls list
            CharactersGridItem[] oldControls = new CharactersGridItem[this.tableLayoutPanel.Controls.Count];
            this.tableLayoutPanel.Controls.CopyTo(oldControls, 0);

            // Clear the controls list
            this.tableLayoutPanel.Controls.Clear();

            // Dispose every one of the control to prevent timer's execution
            foreach (CharactersGridItem item in oldControls)
            {
                item.Stop();
                item.Click -= new EventHandler(item_Click);
            }
        }

        /// <summary>
        /// Adjust the number of columns
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            UpdateLayout();
            base.OnClientSizeChanged(e);
        }

        /// <summary>
        /// Updates the number of rows and columns
        /// </summary>
        private void UpdateLayout()
        {
            int numControls = tableLayoutPanel.Controls.Count;
            if (numControls == 0) return;

            // Computes the number of columns we need
            int numColumns = Math.Max(1, Math.Min(numControls, this.ClientSize.Width / WidthPerCharacter));
            tableLayoutPanel.ColumnCount = numColumns;

            // Computes the number of rows we need
            int numRows = (tableLayoutPanel.Controls.Count + numColumns - 1) / numColumns;
            tableLayoutPanel.RowCount = numRows;

            // Updates the rows' heights
            tableLayoutPanel.RowStyles.Clear();
            for (int i = 0; i < numRows; i++)
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            // Updates the columns' widthes
            tableLayoutPanel.ColumnStyles.Clear();
            for (int i = 0; i < numColumns; i++)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, WidthPerCharacter));
            }

            // Updates the table's margins to center it
            int neededWith = numColumns * WidthPerCharacter;
            int neededHeight = numRows * (tableLayoutPanel.Controls[0].Height);
            int marginH = Math.Max(0, (this.ClientSize.Width - neededWith) >> 1);
            int marginV = Math.Max(0, (this.ClientSize.Height - neededHeight) / 3); // We puts 1/3 at the top, 2/3 at the bottom
            tableLayoutPanel.Padding = new Padding(marginH, marginV, marginH, marginV * 2);
        }

        /// <summary>
        /// When an item has been clicked, fires the appropriate event to notify the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item_Click(object sender, EventArgs e)
        {
            var item = sender as CharactersGridItem;
            if (this.CharacterClicked != null && item != null && item.CharacterMonitor != null)
            {
                this.CharacterClicked(this, new CharacterClickedEventArgs { CharacterMonitor = item.CharacterMonitor });
            }

        }

        /// <summary>
        /// On every timer tick, we update the children
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            foreach (CharactersGridItem item in tableLayoutPanel.Controls)
            {
                item.UpdateOnTimerTick();
            }
        }

    }
}
