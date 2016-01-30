using System;
using System.Collections;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Controls
{
    public partial class KillReportAttacker : UserControl
    {
        private SerializableKillLogAttackersListItem m_attacker;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KillReportAttacker"/> class.
        /// </summary>
        public KillReportAttacker()
        {
            InitializeComponent();

            // Set the mouse click event for each control to handle the parent panel scrolling
            SetControlMouseEvents(Controls);
        }


        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the attacker.
        /// </summary>
        /// <value>
        /// The attacker.
        /// </value>
        internal SerializableKillLogAttackersListItem Attacker
        {
            get { return m_attacker; }
            set
            {
                m_attacker = value;
                UpdateContent();
            }
        }

        /// <summary>
        /// Gets or sets the total damage done.
        /// </summary>
        /// <value>
        /// The total damage done.
        /// </value>
        public int TotalDamageDone { get; set; }

        #endregion


        #region Content Management Methods

        /// <summary>
        /// Updates the content.
        /// </summary>
        private void UpdateContent()
        {
            CharacterNameLabel.Text = String.IsNullOrEmpty(m_attacker.Name) ? m_attacker.ShipTypeName : m_attacker.Name;
            CorpNameLabel.Text = m_attacker.CorporationName;
            AllianceNameLabel.Text = m_attacker.AllianceID == 0 ? String.Empty : m_attacker.AllianceName;

            DamageDoneLabel.Text = String.Format(CultureConstants.DefaultCulture, DamageDoneLabel.Text, m_attacker.DamageDone,
                m_attacker.DamageDone / (double)TotalDamageDone);

            Task.WhenAll(
                GetImageForAsync(CharacterPictureBox),
                GetImageForAsync(ShipPictureBox),
                GetImageForAsync(WeaponPictureBox));
        }

        /// <summary>
        /// Gets the image for the specified picture box.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        private async Task GetImageForAsync(PictureBox pictureBox, bool useFallbackUri = false)
        {
            while (true)
            {
                Image img = await ImageService.GetImageAsync(GetImageUrl(pictureBox, useFallbackUri));
                if (img == null && !useFallbackUri)
                {
                    useFallbackUri = true;
                    continue;
                }

                pictureBox.Image = img;
                break;
            }
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        /// <returns></returns>
        private Uri GetImageUrl(PictureBox pictureBox, bool useFallbackUri)
        {
            string path = String.Empty;

            if (pictureBox.Equals(CharacterPictureBox))
            {
                path = String.Format(CultureConstants.InvariantCulture,
                    NetworkConstants.CCPPortraits, m_attacker.ID, (int)EveImageSize.x64);
            }

            if (pictureBox.Equals(ShipPictureBox) || pictureBox.Equals(WeaponPictureBox))
            {
                int typeId = pictureBox.Equals(ShipPictureBox)
                    ? m_attacker.ShipTypeID
                    : m_attacker.WeaponTypeID;

                path = String.Format(CultureConstants.InvariantCulture,
                    NetworkConstants.CCPIconsFromImageServer, "type", typeId, (int)EveImageSize.x32);
            }

            return useFallbackUri
                ? ImageService.GetImageServerBaseUri(path)
                : ImageService.GetImageServerCdnUri(path);
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Sets the control mouse events.
        /// </summary>
        /// <param name="controls">The controls.</param>
        private void SetControlMouseEvents(IEnumerable controls)
        {
            // Give focus to parent panel for mouse wheel scrolling
            foreach (Control control in controls)
            {
                control.MouseClick += control_MouseClick;

                if (control.Controls.Count > 0)
                    SetControlMouseEvents(control.Controls);
            }
        }

        #endregion


        #region Local Events

        /// <summary>
        /// Handles the MouseEnter event of the control control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void control_MouseClick(object sender, MouseEventArgs e)
        {
            Parent.Focus();
        }


        #endregion
    }
}
