using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.Sales
{
    [Serializable]
    public partial class MineralTile : UserControl
    {
        public event EventHandler<EventArgs> SubtotalChanged;
        public event EventHandler<EventArgs> MineralPriceChanged;

        private string m_mineralName;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MineralTile"/> class.
        /// </summary>
        public MineralTile()
        {
            Subtotal = 0;
            InitializeComponent();
        }

        #endregion


        #region Public Properties

        public String MineralName
        {
            get { return m_mineralName; }
            set
            {
                m_mineralName = value;
                groupBox1.Text = value;
                Stream s = null;
                Image i = null;
                try
                {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    s = asm.GetManifestResourceStream("EVEMon.Sales.icons." + value + ".png");
                    if (s != null)
                        i = Image.FromStream(s, true, true);
                    Icon.Image = i;
                }
                catch (Exception e)
                {
                    ExceptionHandler.LogException(e, true);
                    if (i != null)
                        i.Dispose();

                    if (s != null)
                        s.Dispose();

                    Icon.Image = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public PictureBox Icon { get; private set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public int Quantity
        {
            get { return Int32.Parse(txtStock.Text); }
            set { txtStock.Text = value.ToString(); }
        }

        /// <summary>
        /// Gets or sets the price per unit.
        /// </summary>
        /// <value>The price per unit.</value>
        public Decimal PricePerUnit
        {
            get { return Decimal.Parse(txtLastSell.Text); }
            set { txtLastSell.Text = value.ToString("N"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [price locked].
        /// </summary>
        /// <value><c>true</c> if [price locked]; otherwise, <c>false</c>.</value>
        public bool PriceLocked
        {
            get { return txtLastSell.ReadOnly; }
            set
            {
                txtLastSell.TabStop = !value;
                txtLastSell.ReadOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets the subtotal.
        /// </summary>
        /// <value>The subtotal.</value>
        public decimal Subtotal { get; private set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Updates the subtotal.
        /// </summary>
        private void UpdateSubtotal()
        {
            try
            {
                Decimal pricePerUnit = Decimal.Parse(txtLastSell.Text);
                int quantity = Int32.Parse(txtStock.Text);

                Subtotal = pricePerUnit * quantity;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(e, true);
                Subtotal = 0;
            }
            tbSubtotal.Text = Subtotal.ToString("N");

            if (SubtotalChanged != null)
                SubtotalChanged(this, new EventArgs());
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Handles the TextChanged event of the txtLastSell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtLastSell_TextChanged(object sender, EventArgs e)
        {
            UpdateSubtotal();
            if (MineralPriceChanged != null)
                MineralPriceChanged(this, new EventArgs());
        }

        /// <summary>
        /// Handles the TextChanged event of the txtStock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void txtStock_TextChanged(object sender, EventArgs e)
        {
            UpdateSubtotal();
        }

        #endregion
    }
}