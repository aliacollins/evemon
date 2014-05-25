using System;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Data;

namespace EVEMon.Controls
{
    public partial class KillReportVictim : UserControl
    {
        private KillLog m_killLog;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KillReportVictim"/> class.
        /// </summary>
        public KillReportVictim()
        {
            InitializeComponent();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the kill log.
        /// </summary>
        /// <value>
        /// The kill log.
        /// </value>
        internal KillLog KillLog
        {
            get { return m_killLog; }
            set
            {
                m_killLog = value;
                UpdateContent();
            }
        }

        #endregion


        #region Content Management Methods

        /// <summary>
        /// Updates the content.
        /// </summary>
        private void UpdateContent()
        {
            GetImageFor(CharacterPictureBox);
            GetImageFor(ShipPictureBox);
            GetImageFor(CorpPictureBox);

            if (m_killLog.Victim.AllianceID != 0)
            {
                GetImageFor(AlliancePictureBox);
                AllianceNameLabel.Text = m_killLog.Victim.AllianceName;
            }
            else
            {
                CorpAllianceFlowLayoutPanel.Controls.Remove(AllianceNameLabel);
                CorpAllianceFlowLayoutPanel.Padding = new Padding(0, 12, 0, 0);
            }

            CharacterNameLabel.Text = m_killLog.Victim.Name;
            CorpNameLabel.Text = m_killLog.Victim.CorporationName;

            Item ship = StaticItems.GetItemByID(m_killLog.Victim.ShipTypeID);
            ShipNameLabel.Text = ship.Name;
            ShipGroupLabel.Text = String.Format(CultureConstants.DefaultCulture, ShipGroupLabel.Text, ship.GroupName);

            KillTimeLabel.Text = m_killLog.KillTime.ToLocalTime().DateTimeToDotFormattedString();
            SolarSystemLabel.Text = m_killLog.SolarSystem.Name;
            SecStatusLabel.Text = m_killLog.SolarSystem.SecurityLevel.ToNumericString(1);
            SecStatusLabel.ForeColor = m_killLog.SolarSystem.SecurityLevelColor;
            ConstelationLabel.Text = m_killLog.SolarSystem.Constellation.Name;
            RegionLabel.Text = m_killLog.SolarSystem.Constellation.Region.Name;
        }

        /// <summary>
        /// Gets the image for the specified picture box.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        private void GetImageFor(PictureBox pictureBox)
        {
            string url = null;

            if (pictureBox.Equals(CharacterPictureBox))
                url = String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEImageBase,
                    String.Format(CultureConstants.InvariantCulture,
                        NetworkConstants.CCPPortraits, m_killLog.Victim.ID, (int)EveImageSize.x128));

            if (pictureBox.Equals(ShipPictureBox))
                url = String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEImageBase,
                    String.Format(CultureConstants.InvariantCulture,
                        NetworkConstants.CCPIconsFromImageServer, "render", m_killLog.Victim.ShipTypeID,
                        (int)EveImageSize.x128));

            if (pictureBox.Equals(CorpPictureBox))
                url = String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEImageBase,
                    String.Format(CultureConstants.InvariantCulture,
                        NetworkConstants.CCPIconsFromImageServer, "corporation", m_killLog.Victim.CorporationID,
                        (int)EveImageSize.x32));

            if (pictureBox.Equals(AlliancePictureBox))
                url = String.Format(CultureConstants.InvariantCulture, "{0}{1}", NetworkConstants.EVEImageBase,
                    String.Format(CultureConstants.InvariantCulture,
                        NetworkConstants.CCPIconsFromImageServer, "alliance", m_killLog.Victim.AllianceID,
                        (int)EveImageSize.x32));

            if (!String.IsNullOrEmpty(url))
            {
                ImageService.GetImageAsync(new Uri(url), img =>
                {
                    if (img == null)
                        return;

                    pictureBox.Image = img;
                });
            }
        }

        #endregion


        #region Local Events

        /// <summary>
        /// Handles the Click event of the CopyPictureBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CopyPictureBox_Click(object sender, EventArgs e)
        {
            KillLogExporter.CopyKillInfoToClipboard(m_killLog);
        }

        #endregion
    }
}
