using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.Sales
{
    internal delegate void TileUpdate(MineralTile mt, Single s);

    public partial class MineralWorksheet : EVEMonForm
    {
        private EventHandler<EventArgs> tileChangeHandler;
        private Settings m_settings;

        public MineralWorksheet()
        {
            InitializeComponent();
        }

        public MineralWorksheet(Settings s)
            : this()
        {
            m_settings = s;
        }

        private IEnumerable<MineralTile> Tiles
        {
            get
            {
                foreach (Control c in this.MineralTileTableLayout.Controls)
                {
                    if (c is MineralTile)
                    {
                        yield return c as MineralTile;
                    }
                }
            }
        }

        private void TileSubtotalChanged(object sender, EventArgs e)
        {
            Decimal total = 0;
            foreach (MineralTile mt in Tiles)
            {
                total += mt.Subtotal;
            }
            tslTotal.Text = String.Format("{0} ISK", total.ToString("N"));
        }

        private void MineralWorksheet_Load(object sender, EventArgs e)
        {
            tileChangeHandler = new EventHandler<EventArgs>(TileSubtotalChanged);
            foreach (MineralTile mt in Tiles)
            {
                mt.SubtotalChanged += tileChangeHandler;
            }

            SortedList<string, Pair<string, string>> parsersSorted = new SortedList<string, Pair<string, string>>();

            foreach (Pair<string, IMineralParser> p in MineralDataRequest.Parsers)
            {
                parsersSorted.Add(p.B.Title, new Pair<string, string>(p.A, p.B.Title));
            }

            foreach (Pair<string, string> p in parsersSorted.Values)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem();
                mi.Text = p.B;
                mi.Tag = p.A;
                mi.Click += new EventHandler(mi_Click);
                tsddFetch.DropDownItems.Add(mi);
            }
        }

        private string m_courtesyUrl = String.Empty;

        private void mi_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mi = (ToolStripMenuItem) sender;
            string s = (string) mi.Tag;

            Dictionary<string, Decimal> prices = new Dictionary<string, decimal>();
            try
            {
                foreach (Pair<string, Decimal> p in MineralDataRequest.GetPrices(s))
                {
                    prices[p.A] = p.B;
                }
            }
            catch (MineralParserException mpe)
            {
                ExceptionHandler.LogException(mpe, true);
                MessageBox.Show("Failed to retrieve mineral pricing data:\n" + mpe.Message,
                                "Failed to Retrieve Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (MineralTile mt in Tiles)
            {
                if (prices.ContainsKey(mt.MineralName))
                {
                    mt.PricePerUnit = prices[mt.MineralName];
                }
            }

            tslCourtesy.Text = "Mineral Prices Courtesy of " + MineralDataRequest.GetCourtesyText(s);
            m_courtesyUrl = MineralDataRequest.GetCourtesyUrl(s);
            tslCourtesy.Visible = true;
        }

        private bool m_pricesLocked = false;

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

        private void tslCourtesy_Click(object sender, EventArgs e)
        {
            try
            {
                Process p = Process.Start(m_courtesyUrl);
                p.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogException(ex, false);
            }
        }

        private void mt_MineralPriceChanged(object sender, EventArgs e)
        {
            tslCourtesy.Visible = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            foreach (MineralTile mt in Tiles)
            {
                mt.Quantity=0;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach (MineralTile mt in Tiles)
            {
                mt.Quantity=0;
            }
        }

        String formattedText;
        String unformattedText;

        private void copyTotalDropDownButton_DropDownOpening(object sender, EventArgs e)
        {
            Decimal total = 0;
            foreach (MineralTile mt in Tiles)
            {
                total += mt.Subtotal;
            }

            formattedText = total.ToString("N") + " ISK";
            unformattedText = total.ToString();
            copyFormattedTotalToolStripMenuItem.Text = "Formatted (" + formattedText + ")";
            copyUnformattedTotalToolStripMenuItem.Text = "Unformatted (" + unformattedText + ")";
        }

        private void copyFormattedTotalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(formattedText);
        }

        private void copyUnformattedTotalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(unformattedText);
        }
    }
}
