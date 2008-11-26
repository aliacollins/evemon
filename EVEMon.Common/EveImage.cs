using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace EVEMon.Common
{
    /// <summary>
    /// Displays an image for a given EveObject
    /// </summary>
    /// <remarks>
    /// Setting the PopUpEnabled property to true enables a pop-up window for EveObjects with a 256 x 256 image available, accessed
    /// via the user double-clicking the image.
    /// Image size must be set using the ImageSize property. The default Size property is overriden.
    /// </remarks>
    public partial class EveImage : UserControl
    {

        #region Private Properties
        private bool m_PopUpEnabled;
        private bool m_PopUpActive;
        private EveImageSize m_ImageSize;
        private EveObject m_EveItem = null;
        private const string m_EveSite = "http://www.eve-online.com";
        #endregion

        #region Public Properties
        public enum EveImageSize { _0_0 = 0, _16_16 = 16, _32_32 = 32, _64_64 = 64, _128_128 = 128, _256_256 = 256 }

        public bool PopUpEnabled
        {
            get { return m_PopUpEnabled; }
            set
            {
                m_PopUpEnabled = value;
            }
        }

        public EveObject EveItem
        {
            get { return m_EveItem; }
            set
            {
                m_EveItem = value;
                if (m_ImageSize != EveImageSize._0_0)
                {
                    GetImage();
                }
            }
        }

        public EveImageSize ImageSize
        {
            get { return m_ImageSize; }
            set
            {
                m_ImageSize = value;
                pbImage.Size = new Size((int)m_ImageSize, (int)m_ImageSize);
                this.Size = pbImage.Size;
            }
        }

        new public Size Size
        {
            get { return base.Size; }
            set
            {
                base.Size = new Size((int)m_ImageSize, (int)m_ImageSize);
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initialise the control
        /// </summary>
        /// <remarks>
        /// The default image size is 64 x 64, with the image pop-up enabled.
        /// </remarks>
        public EveImage()
        {
            InitializeComponent();
            SetImageTypeAttributes();
            ImageSize = EveImageSize._64_64;
            PopUpEnabled = true;
            ShowBlankImage();
        }

        #endregion

        #region Image type configuration
        /// <summary>
        /// Identifies the image type being handled
        /// </summary>
        private enum ImageType { Ship, Drone, Structure, Item, None }

        /// <summary>
        /// Indicates the source of the .png image name
        /// </summary>
        private enum ImageNameFrom { TypeID, Icon };

        /// <summary>
        /// Holds configuration data for different image types
        /// </summary>
        private Dictionary<ImageType, ImageTypeData> m_ImageTypeAttributes;

        /// <summary>
        /// Defines configuration data for a specific ImageType
        /// </summary>
        private struct ImageTypeData
        {
            public string localComponent;
            public string urlPath;
            public ImageNameFrom imageNameFrom;
            public ArrayList validSizes;

            public ImageTypeData(string local, string url, ImageNameFrom name, ArrayList sizes)
            {
                localComponent = local;
                urlPath = url;
                imageNameFrom = name;
                validSizes = sizes;
            }
        }

        /// <summary>
        /// Builds the m_ImageTypeAttributes dictionary
        /// </summary>
        private void SetImageTypeAttributes()
        {
            ArrayList validSizes;
            m_ImageTypeAttributes = new Dictionary<ImageType, ImageTypeData>();
            // Ships
            validSizes = new ArrayList();
            validSizes.Add(EveImageSize._32_32);
            validSizes.Add(EveImageSize._64_64);
            validSizes.Add(EveImageSize._128_128);
            validSizes.Add(EveImageSize._256_256);
            m_ImageTypeAttributes.Add(ImageType.Ship, new ImageTypeData("Ships", "bitmaps/icons/itemdb/shiptypes", ImageNameFrom.TypeID, validSizes));
            // Items
            validSizes = new ArrayList();
            validSizes.Add(EveImageSize._16_16);
            validSizes.Add(EveImageSize._32_32);
            validSizes.Add(EveImageSize._64_64);
            validSizes.Add(EveImageSize._128_128);
            m_ImageTypeAttributes.Add(ImageType.Item, new ImageTypeData("Items", "bitmaps/icons/itemdb/black", ImageNameFrom.Icon, validSizes));
            // Drones
            validSizes = new ArrayList();
            validSizes.Add(EveImageSize._32_32);
            validSizes.Add(EveImageSize._64_64);
            validSizes.Add(EveImageSize._128_128);
            validSizes.Add(EveImageSize._256_256);
            m_ImageTypeAttributes.Add(ImageType.Drone, new ImageTypeData("Drones", "bitmaps/icons/itemdb/dronetypes", ImageNameFrom.TypeID, validSizes));
            // Structures
            validSizes = new ArrayList();
            validSizes.Add(EveImageSize._32_32);
            validSizes.Add(EveImageSize._64_64);
            validSizes.Add(EveImageSize._128_128);
            validSizes.Add(EveImageSize._256_256);
            m_ImageTypeAttributes.Add(ImageType.Structure, new ImageTypeData("", "bitmaps/icons/itemdb/structuretypes", ImageNameFrom.TypeID, validSizes));
        }

        private ImageType GetImageType(EveObject EveItem)
        {
            ImageType imageType = ImageType.None;
            if (EveItem.GetType() == typeof(Ship))
            {
                imageType = ImageType.Ship;
            }
            else if (EveItem.GetType() == typeof(Item))
            {
                Item item = (Item)EveItem;
                ItemCategory baseCat = item.ParentCategory;
                while (baseCat.ParentCategory.Name != "Ship Items")
                {
                    baseCat = baseCat.ParentCategory;
                }
                if (baseCat.Name == "Drones" && item.ParentCategory.Name != "Drone Upgrades")
                {
                    imageType = ImageType.Drone;
                }
                else if (baseCat.Name == "Starbase Structures")
                {
                    imageType = ImageType.Structure;
                }
                else
                {
                    imageType = ImageType.Item;
                }
            }
            return imageType;
        }

        #endregion

        #region Image Retrieval and Pop Up
        /// <summary>
        /// Renders a BackColor square as a placeholder for the image
        /// </summary>
        private void ShowBlankImage()
        {
            Bitmap b = new Bitmap(pbImage.ClientSize.Width, pbImage.ClientSize.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, b.Width, b.Height));
            }
            pbImage.Image = b;
        }

        /// <summary>
        /// Retrieves image for the given EveObject
        /// </summary>
        private void GetImage()
        {
            // Set to black image initially
            ShowBlankImage();

            // Restet flags and cursor
            m_PopUpActive = false;
            toolTip1.Active = false;
            pbImage.Cursor = Cursors.Default;

            if (m_EveItem != null)
            {
                ImageType imageType = GetImageType(m_EveItem);
                ImageTypeData typeData = m_ImageTypeAttributes[imageType];

                // Only display an image if the cirrect size is available
                if (typeData.validSizes.Contains(m_ImageSize))
                {
                    // Set file & pathname variables
                    string eveSize = m_ImageSize.ToString().Substring(1);
                    string imageWebName;
                    string imageResourceName;
                    if (typeData.imageNameFrom == ImageNameFrom.TypeID)
                    {
                        imageWebName = m_EveItem.Id.ToString();
                        imageResourceName = "_" + imageWebName;
                    }
                    else
                    {
                        imageWebName = m_EveItem.Icon.ToString().Substring(m_EveItem.Icon.ToString().LastIndexOf('/') + 1, m_EveItem.Icon.ToString().Substring(m_EveItem.Icon.ToString().LastIndexOf('/') + 1).Length - 4);
                        imageResourceName = imageWebName;
                    }

                    // Try and get image from local resources
                    string localResources = String.Format(
                        "{1}Resources{0}Optional{0}{2}{3}.resources",
                        Path.DirectorySeparatorChar,
                        System.AppDomain.CurrentDomain.BaseDirectory,
                        typeData.localComponent,
                        eveSize);
                    if (System.IO.File.Exists(localResources))
                    {
                        System.Resources.IResourceReader basic;
                        basic = new System.Resources.ResourceReader(localResources);
                        System.Collections.IDictionaryEnumerator basicx = basic.GetEnumerator();
                        while (basicx.MoveNext())
                        {
                            if (basicx.Key.ToString() == imageResourceName)
                            {
                                pbImage.Image = (System.Drawing.Image)basicx.Value;
                            }
                        }
                    }
                    else
                    // Try to get image from web
                    {
                        string imageURL = String.Format("{0}/{1}/{2}/{3}.png",
                            m_EveSite,
                            typeData.urlPath,
                            eveSize,
                            imageWebName);
                        ImageService.GetImageAsync(imageURL, true, delegate(EveSession ss, Image i)
                                                                  {
                                                                      GotImage(m_EveItem.Id, i);
                                                                  });
                    }

                    // Enable pop up if required
                    if (m_PopUpEnabled && typeData.validSizes.Contains(EveImageSize._256_256))
                    {
                        toolTip1.Active = true;
                        m_PopUpActive = true;
                        pbImage.Cursor = Cursors.Hand;
                    }

                }
            }
        }

        /// <summary>
        /// Callback method for asynchronous web requests
        /// </summary>
        /// <param name="id">EveObject id for retrieved image</param>
        /// <param name="i">Image object retrieved</param>
        private void GotImage(int id, Image i)
        {
            // Only display the image if the id matches the current EveObject
            if (i != null && m_EveItem.Id == id)
            {
                pbImage.Image = i;
            }
        }

        /// <summary>
        /// Event handler for image double click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbImage_DoubleClick(object sender, EventArgs e)
        {
            // Only display the pop up form if pop-ups are enabled and a suitable image can be retrieved
            if (m_PopUpActive)
            {
                EveImagePopUp popup = new EveImagePopUp(m_EveItem);
                popup.FormClosed += delegate { popup.Dispose(); };
                popup.Show();
            }
        }
        #endregion

    }
}
