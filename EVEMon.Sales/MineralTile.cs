using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.Sales
{
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
            InitializeComponent();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the mineral.
        /// </summary>
        /// <value>The name of the mineral.</value>
        public String MineralName
        {
            get { return m_mineralName; }
            set
            {
                m_mineralName = value;
                groupBox.Text = value;
                SetIconByName(value);
            }
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public long Quantity
        {
            get { return Int64.Parse(txtStock.Text, CultureConstants.DefaultCulture); }
            set { txtStock.Text = value.ToString(CultureConstants.DefaultCulture); }
        }

        /// <summary>
        /// Gets or sets the price per unit.
        /// </summary>
        /// <value>The price per unit.</value>
        public Decimal PricePerUnit
        {
            get { return Decimal.Parse(txtLastSell.Text, CultureConstants.DefaultCulture); }
            set { txtLastSell.Text = value.ToString("N", CultureConstants.DefaultCulture); }
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
        /// Sets the icon by mineral name.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetIconByName(string value)
        {
            Stream stream = null;
            Image image = null;

            try
            {
                stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EVEMon.Sales.icons." + value + ".png");
                if (stream != null)
                    image = Image.FromStream(stream, true, true);

                icon.Image = image;
            }
            catch (BadImageFormatException e)
            {
                ExceptionHandler.LogException(e, true);
                if (image != null)
                    image.Dispose();

                if (stream != null)
                    stream.Dispose();

                icon.Image = null;
            }
        }

        /// <summary>
        /// Updates the subtotal.
        /// </summary>
        private void UpdateSubtotal()
        {
            decimal pricePerUnit;
            long quantity;
            if (!Decimal.TryParse(txtLastSell.Text, out pricePerUnit))
                pricePerUnit = 0;

            if (!Int64.TryParse(txtStock.Text, out quantity))
                quantity = 0;

            Subtotal = pricePerUnit * quantity;

            tbSubtotal.Text = Subtotal.ToString("N", CultureConstants.DefaultCulture);

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