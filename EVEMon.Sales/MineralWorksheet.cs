using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.Sales
{
    public partial class MineralWorksheet : EVEMonForm
    {
        private Decimal m_total;
        private string m_courtesyUrl;
        private string m_source;
        private bool m_pricesLocked;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MineralWorksheet"/> class.
        /// </summary>
        public MineralWorksheet()
        {
            InitializeComponent();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the tiles.
        /// </summary>
        /// <value>The tiles.</value>
        private IEnumerable<MineralTile> Tiles
        {
            get { return MineralTileTableLayout.Controls.OfType<MineralTile>().Select(control => control); }
        }

        /// <summary>
        /// Sets a value indicating whether [prices locked].
        /// </summary>
        /// <value><c>true</c> if [prices locked]; otherwise, <c>false</c>.</value>
        private bool PricesLocked
        {
            set
            {
                m_pricesLocked = value;
                foreach (MineralTile mt in Tiles)
                {
                    mt.PriceLocked = value;
                }
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Updates the visuals.
        /// </summary>
        private void UpdateVisuals()
        {
            foreach (ToolStripItem item in MineralWorksheetToolStrip.Items)
            {
                item.DisplayStyle = (Settings.UI.SafeForWork
                                         ? ToolStripItemDisplayStyle.Text
                                         : ToolStripItemDisplayStyle.ImageAndText);
            }

            foreach (ToolStripStatusLabel item in MineralWorksheetStatusStrip.Items)
            {
                item.DisplayStyle = (Settings.UI.SafeForWork
                                         ? ToolStripItemDisplayStyle.Text
                                         : ToolStripItemDisplayStyle.ImageAndText);
            }

            foreach (MineralTile tile in Tiles)
            {
                tile.Icon.Visible = !Settings.UI.SafeForWork;
            }
        }

        #endregion


        #region Local Events Handlers

        /// <summary>
        /// Handles the Load event of the MineralWorksheet control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MineralWorksheet_Load(object sender, EventArgs e)
        {
            EveMonClient.SettingsChanged += EveMonClient_SettingsChanged;
            Disposed += OnDiposed;

            UpdateVisuals();

            foreach (MineralTile mt in Tiles)
            {
                mt.SubtotalChanged += TileSubtotalChanged;
            }

            SortedList<string, Pair<string, string>> parsersSorted = new SortedList<string, Pair<string, string>>();

            foreach (Pair<string, IMineralParser> p in MineralDataRequest.Parsers)
            {
                parsersSorted.Add(p.B.Title, new Pair<string, string>(p.A, p.B.Title));
            }

            foreach (ToolStripMenuItem mi in parsersSorted.Values.Select(p => new ToolStripMenuItem { Text = p.B, Tag = p.A }))
            {
                mi.Click += mi_Click;
                tsddFetch.DropDownItems.Add(mi);
            }
        }

        /// <summary>
        /// Called when [diposed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDiposed(object sender, EventArgs e)
        {
            EveMonClient.SettingsChanged -= EveMonClient_SettingsChanged;
            Disposed -= OnDiposed;
        }

        /// <summary>
        /// Tiles the subtotal changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TileSubtotalChanged(object sender, EventArgs e)
        {
            m_total = 0;
            foreach (MineralTile mt in Tiles)
            {
                m_total += mt.Subtotal;
            }

            tslTotal.Text = String.Format(CultureConstants.DefaultCulture, "{0:N} ISK", m_total);
        }

        #endregion


        #region Global Event Handlers

        /// <summary>
        /// Handles the SettingsChanged event of the EveMonClient control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void EveMonClient_SettingsChanged(object sender, EventArgs e)
        {
            UpdateVisuals();
        }

        #endregion


        #region Control Event Handlers

        /// <summary>
        /// Handles the Click event of the mi control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mi_Click(object sender, EventArgs e)
        {
            if (bckgrndWrkrGetPrices.IsBusy)
                return;

            ToolStripMenuItem mi = sender as ToolStripMenuItem;
            if (mi == null || !(mi.Tag is string))
                return;

            m_source = mi.Tag as string;
            bckgrndWrkrGetPrices.RunWorkerAsync(m_source);
        }

        /// <summary>
        /// Handles the DoWork event of the bckgrndWrkrGetPrices control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        private void bckgrndWrkrGetPrices_DoWork(object sender, DoWorkEventArgs e)
        {
            IDictionary<string, Decimal> prices = new Dictionary<string, decimal>();
            string source = e.Argument as string;
            if (source != null)
            {
                foreach (Pair<string, Decimal> p in MineralDataRequest.GetPrices(source))
                {
                    prices[p.A] = p.B;
                }
            }

            e.Result = prices;
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the bckgrndWrkrGetPrices control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void bckgrndWrkrGetPrices_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ExceptionHandler.LogException(e.Error, true);
                MessageBox.Show(String.Format("Failed to retrieve mineral pricing data:\n{0}", e.Error.Message),
                                "Failed to Retrieve Data",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                IDictionary<string, Decimal> prices = e.Result as IDictionary<string, Decimal>;
                if (prices != null)
                {
                    foreach (MineralTile mt in Tiles.Where(mt => prices.ContainsKey(mt.MineralName)))
                    {
                        mt.PricePerUnit = prices[mt.MineralName];
                    }

                    tslCourtesy.Text = String.Format("Mineral Prices Courtesy of {0}",
                                                     MineralDataRequest.GetCourtesyText(m_source));
                    m_courtesyUrl = MineralDataRequest.GetCourtesyUrl(m_source);
                    tslCourtesy.Visible = true;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnLockPrices control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnLockPrices_Click(object sender, EventArgs e)
        {
            if (m_pricesLocked)
            {
                PricesLocked = false;
                btnLockPrices.Text = "Lock Prices";
            }
            else
            {
                PricesLocked = true;
                btnLockPrices.Text = "Unlock Prices";
            }
        }

        /// <summary>
        /// Handles the Click event of the tslCourtesy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void tslCourtesy_Click(object sender, EventArgs e)
        {
            try
            {
                using (Process.Start(m_courtesyUrl))
                {
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, false);
            }
        }

        /// <summary>
        /// Handles the MineralPriceChanged event of the mt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mt_MineralPriceChanged(object sender, EventArgs e)
        {
            tslCourtesy.Visible = false;
        }

        /// <summary>
        /// Handles the Click event of the btnReset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach (MineralTile mt in Tiles)
            {
                mt.Quantity = 0;
            }
        }

        /// <summary>
        /// Handles the DropDownOpening event of the copyTotalDropDownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void copyTotalDropDownButton_DropDownOpening(object sender, EventArgs e)
        {
            copyFormattedTotalToolStripMenuItem.Text = String.Format(CultureConstants.DefaultCulture, "Formatted ({0:N} ISK)",
                                                                     m_total);
            copyUnformattedTotalToolStripMenuItem.Text = String.Format(CultureConstants.DefaultCulture, "Unformatted ({0})",
                                                                       m_total);
        }

        /// <summary>
        /// Handles the Click event of the copyFormattedTotalToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void copyFormattedTotalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(tslTotal.Text);
            }
            catch (ExternalException ex)
            {
                // there is a bug that results in an exception being
                // thrown when the clipboard is in use by another
                // thread.
                ExceptionHandler.LogException(ex, true);
            }
        }

        /// <summary>
        /// Handles the Click event of the copyUnformattedTotalToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void copyUnformattedTotalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(m_total.ToString());
            }
            catch (ExternalException ex)
            {
                // there is a bug that results in an exception being
                // thrown when the clipboard is in use by another
                // thread.
                ExceptionHandler.LogException(ex, true);
            }
        }

        #endregion
    }
}