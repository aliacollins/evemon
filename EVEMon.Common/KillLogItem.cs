using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

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
        /// <param name="src">The SRC.</param>
        public KillLogItem(SerializableKillLogItemListItem src)
        {
            m_typeID = src.TypeID;
            EVEFlag = src.EVEFlag;
            QtyDestroyed = src.QtyDestroyed;
            QtyDropped = src.QtyDropped;
            Singleton = src.Singleton;

            m_items.AddRange(src.Items.Select(item => new KillLogItem(item)));
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
        /// Gets the items.
        /// </summary>
        public IEnumerable<KillLogItem> Items
        {
            get { return m_items; }
        }

        /// <summary>
        /// Gets the inventory text.
        /// </summary>
        public string InventoryText
        {
            get { return EveFlag.GetFlagText(EVEFlag); }
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
        /// Gets the group text.
        /// </summary>
        /// <returns></returns>
        public string GroupText
        {
            get
            {
                switch (EVEFlag)
                {
                    case 0:
                        return "Other";
                    case 5:
                        return "Cargo Bay";
                    case 89:
                        return "Implants";
                    default:
                        return EveFlag.GetFlagText(EVEFlag);
                }
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
        /// Gets the item image.
        /// </summary>
        private void GetItemImage()
        {
            m_image = new Bitmap(24, 24);
            ImageService.GetImageAsync(GetImageUrl(), img =>
                                                          {
                                                              if (img == null)
                                                                  return;

                                                              m_image = img;

                                                              // Notify the subscriber that we got the image
                                                              if (KillLogItemImageUpdated != null)
                                                                  KillLogItemImageUpdated(this, EventArgs.Empty);
                                                          });
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <returns></returns>
        private Uri GetImageUrl()
        {
            return new Uri(String.Format(CultureConstants.InvariantCulture,
                                         NetworkConstants.CCPIconsFromImageServer, "type", m_typeID,
                                         (int)EveImageSize.x32));
        }

        #endregion

    }
}