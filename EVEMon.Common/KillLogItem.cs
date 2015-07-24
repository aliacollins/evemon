using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;
using EVEMon.Common.Serialization.BattleClinic.MarketPrices;

namespace EVEMon.Common
{
    public class KillLogItem
    {
        /// <summary>
        /// Occurs when kill log item image updated.
        /// </summary>
        public event EventHandler KillLogItemImageUpdated;


        #region Fields

        private readonly List<KillLogItem> m_items = new List<KillLogItem>();
        private readonly int m_typeID;
        private Image m_image;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KillLogItem"/> class.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="isInContainer">if set to <c>true</c> item is in container.</param>
        internal KillLogItem(SerializableKillLogItemListItem src, bool isInContainer = false)
        {
            m_typeID = src.TypeID;
            EVEFlag = src.EVEFlag;
            QtyDestroyed = src.QtyDestroyed;
            QtyDropped = src.QtyDropped;
            Singleton = src.Singleton;

            FittingContentGroup = GetFittingContentGroup();
            IsInContainer = isInContainer;

            m_items.AddRange(src.Items.Select(item => new KillLogItem(item, true)));
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the EVE flag.
        /// </summary>
        public short EVEFlag { get; private set; }

        /// <summary>
        /// Gets the qty dropped.
        /// </summary>
        public int QtyDropped { get; private set; }

        /// <summary>
        /// Gets the qty destroyed.
        /// </summary>
        public int QtyDestroyed { get; private set; }

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        public byte Singleton { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the item is in a container.
        /// </summary>
        public bool IsInContainer { get; private set; }

        /// <summary>
        /// Gets the fitting content group.
        /// </summary>
        public KillLogFittingContentGroup FittingContentGroup { get; private set; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IEnumerable<KillLogItem> Items
        {
            get { return m_items; }
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public double Price
        {
            get { return BCItemPrices.GetPriceByTypeID(m_typeID); }
        }

        /// <summary>
        /// Gets the inventory text.
        /// </summary>
        public string InventoryText
        {
            get
            {
                switch (FittingContentGroup)
                {
                    case KillLogFittingContentGroup.Implant:
                    case KillLogFittingContentGroup.DroneBay:
                    case KillLogFittingContentGroup.Cargo:
                        return EveFlag.GetFlagText(EVEFlag);
                    default:
                        return String.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                Item item = StaticItems.GetItemByID(m_typeID);
                return item == null ? "Unknown" : item.Name;
            }
        }

        /// <summary>
        /// Gets the victim image.
        /// </summary>
        public Image ItemImage
        {
            get
            {
                if (m_image == null)
                    GetItemImage();

                return m_image;
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the fiiting and content group.
        /// </summary>
        /// <returns></returns>
        private KillLogFittingContentGroup GetFittingContentGroup()
        {
            switch (EVEFlag)
            {
                case 0:
                    return KillLogFittingContentGroup.Other;
                case 5:
                    return KillLogFittingContentGroup.Cargo;
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                    return KillLogFittingContentGroup.LowSlot;
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                    return KillLogFittingContentGroup.MediumSlot;
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                    return KillLogFittingContentGroup.HighSlot;
                case 87:
                    return KillLogFittingContentGroup.DroneBay;
                case 88:
                    return KillLogFittingContentGroup.Booster;
                case 89:
                    return KillLogFittingContentGroup.Implant;
                case 92:
                case 93:
                case 94:
                case 95:
                case 96:
                case 97:
                case 98:
                case 99:
                    return KillLogFittingContentGroup.RigSlot;
                case 125:
                case 126:
                case 127:
                case 128:
                case 129:
                case 130:
                case 131:
                case 132:
                    return KillLogFittingContentGroup.SubsystemSlot;
                default:
                    return KillLogFittingContentGroup.None;
            }
        }

        /// <summary>
        /// Gets the item image.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        private void GetItemImage(bool useFallbackUri = false)
        {
            m_image = GetDefaultImage();
            ImageService.GetImageAsync(GetImageUrl(useFallbackUri), img =>
            {
                if (img == null)
                {
                    GetItemImage(true);
                    return;
                }

                m_image = img;

                // Notify the subscriber that we got the image
                if (KillLogItemImageUpdated != null)
                    KillLogItemImageUpdated(this, EventArgs.Empty);
            });
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <returns></returns>
        private static Bitmap GetDefaultImage()
        {
            return new Bitmap(24, 24);
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        /// <returns></returns>
        private Uri GetImageUrl(bool useFallbackUri)
        {
            string path = String.Format(CultureConstants.InvariantCulture,
                NetworkConstants.CCPIconsFromImageServer, "type", m_typeID,
                (int)EveImageSize.x32);

            return useFallbackUri
                ? ImageService.GetImageServerBaseUri(path)
                : ImageService.GetImageServerCdnUri(path);
        }

        #endregion

    }
}