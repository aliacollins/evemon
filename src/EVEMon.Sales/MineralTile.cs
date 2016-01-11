using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Service;

namespace EVEMon.Sales
{
    public partial class MineralTile : UserControl
    {
        public event EventHandler<EventArgs> SubtotalChanged;
        public event EventHandler<EventArgs> MineralPriceChanged;

        private Item m_mineral;
        private EveImageSize m_imageSize = EveImageSize.x64;


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
            get { return m_mineral?.Name ?? groupBox.Text; }
            set
            {
                groupBox.Text = value;

                if (DesignMode || this.IsDesignModeHosted())
                    return;

                m_mineral = StaticItems.GetItemByName(value);
                GetImageFromCCP();
            }
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        [Browsable(false)]
        public long Quantity
        {
            get { return Int64.Parse(txtStock.Text, CultureConstants.DefaultCulture); }
            set { txtStock.Text = value.ToString(CultureConstants.DefaultCulture); }
        }

        /// <summary>
        /// Gets or sets the price per unit.
        /// </summary>
        /// <value>The price per unit.</value>
        [Browsable(false)]
        public Decimal PricePerUnit
        {
            get { return Decimal.Parse(txtLastSell.Text, CultureConstants.DefaultCulture); }
            set { txtLastSell.Text = value.ToString("N", CultureConstants.DefaultCulture); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [price locked].
        /// </summary>
        /// <value><c>true</c> if [price locked]; otherwise, <c>false</c>.</value>
        [Browsable(false)]
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
        [Browsable(false)]
        public decimal Subtotal { get; private set; }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the image from CCP's image server.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        private void GetImageFromCCP(bool useFallbackUri = false)
        {
            ImageService.GetImageAsync(GetImageUrl(useFallbackUri), img =>
            {
                if (img == null && !useFallbackUri)
                {
                    GetImageFromCCP(true);
                    return;
                }

                GotImage(m_mineral.ID, img);
            });
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        /// <returns></returns>
        private Uri GetImageUrl(bool useFallbackUri)
        {
            string path = String.Format(CultureConstants.InvariantCulture,
                NetworkConstants.CCPIconsFromImageServer,
                (int)m_imageSize > 64 ? "render" : "type",
                m_mineral.ID, (int)m_imageSize);

            return useFallbackUri
                ? ImageService.GetImageServerBaseUri(path)
                : ImageService.GetImageServerCdnUri(path);
        }

        /// <summary>
        /// Callback method for asynchronous web requests.
        /// </summary>
        /// <param name="id">EveObject id for retrieved image</param>
        /// <param name="image">Image object retrieved</param>
        private void GotImage(long id, Image image)
        {
            // Only display the image if the id matches the current EveObject
            if (image != null && m_mineral.ID == id)
                icon.Image = image;
            else
                ShowBlankImage();
        }

        /// <summary>
        /// Renders a BackColor square as a placeholder for the image.
        /// </summary>
        private void ShowBlankImage()
        {
            Bitmap bmp;
            using (Bitmap tempBitmap = new Bitmap(icon.ClientSize.Width, icon.ClientSize.Height))
            {
                bmp = (Bitmap)tempBitmap.Clone();
            }

            using (Graphics g = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(brush, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            icon.Image = bmp;
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