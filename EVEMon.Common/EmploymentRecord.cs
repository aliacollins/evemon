using System;
using System.Drawing;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EmploymentRecord
    {
        public event EventHandler EmploymentRecordImageUpdated;


        #region Fields

        private readonly Character m_character;
        private readonly long m_corporationId;

        private string m_corporationName;
        private Image m_image;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        public EmploymentRecord(Character character, SerializableEmploymentHistoryListItem src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            m_character = character;
            m_corporationId = src.CorporationID;
            m_corporationName = String.IsNullOrWhiteSpace(src.CorporationName)
                ? GetIDToName(src.CorporationID)
                : src.CorporationName;
            StartDate = src.StartDate;
        }

        /// <summary>
        /// Constructor from the settings.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        public EmploymentRecord(Character character, SerializableEmploymentHistory src)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            m_character = character;
            m_corporationId = src.CorporationID;
            m_corporationName = src.CorporationName;
            StartDate = src.StartDate;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the corporation.
        /// </summary>
        /// <value>The name of the corporation.</value>
        public string CorporationName
        {
            get
            {
                return m_corporationName == EVEMonConstants.UnknownText
                    ? m_corporationName = GetIDToName(m_corporationId)
                    : m_corporationName;

            }
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Gets the corporation image.
        /// </summary>
        /// <value>The corporation image.</value>
        public Image CorporationImage
        {
            get
            {
                if (m_image == null)
                    GetImage();

                return m_image;
            }
        }

        #endregion


        #region Helper Method

        /// <summary>
        /// Gets the corporation name from the provided ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private static string GetIDToName(long id)
        {
            string corporationName = String.Empty;

            // Check if it's an NPC Corporation
            if (id > Int32.MaxValue)
                return String.IsNullOrEmpty(corporationName) ? EveIDToName.GetIDToName(id) : corporationName;

            int npcCorpID = Convert.ToInt32(id);
            NPCCorporation corporation = StaticGeography.GetCorporationByID(npcCorpID);
            corporationName = corporation != null ? corporation.Name : String.Empty;

            // If it's a player's corporation, query the API
            return String.IsNullOrEmpty(corporationName) ? EveIDToName.GetIDToName(id) : corporationName;
        }

        /// <summary>
        /// Gets the corporation image.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        private void GetImage(bool useFallbackUri = false)
        {
            m_image = Properties.Resources.DefaultCorporationImage32;
            ImageService.GetImageAsync(GetImageUrl(useFallbackUri), img =>
            {
                if (img == null)
                {
                    GetImage(true);
                    return;
                }

                m_image = img;

                // Notify the subscriber that we got the image
                // Note that if the image is in cache the event doesn't get fired
                // as the event object is null
                if (EmploymentRecordImageUpdated != null)
                    EmploymentRecordImageUpdated(this, EventArgs.Empty);
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
                NetworkConstants.CCPIconsFromImageServer, "corporation", m_corporationId, (int)EveImageSize.x32);

            return useFallbackUri
                ? ImageService.GetImageServerBaseUri(path)
                : ImageService.GetImageServerCdnUri(path);
        }


        #endregion


        #region Export Method

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        public SerializableEmploymentHistory Export()
        {
            SerializableEmploymentHistory serial = new SerializableEmploymentHistory
            {
                CorporationID = m_corporationId,
                CorporationName = CorporationName,
                StartDate = StartDate
            };
            return serial;
        }

        #endregion

    }
}