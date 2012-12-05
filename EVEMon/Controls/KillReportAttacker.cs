using System;
using System.Collections;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Serialization.API;

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
            GetImageFor(CharacterPictureBox);
            GetImageFor(ShipPictureBox);
            GetImageFor(WeaponPictureBox);

            CharacterNameLabel.Text = String.IsNullOrEmpty(m_attacker.Name) ? m_attacker.ShipTypeName : m_attacker.Name;
            CorpNameLabel.Text = m_attacker.CorporationName;
            AllianceNameLabel.Text = m_attacker.AllianceID == 0 ? String.Empty : m_attacker.AllianceName;

            DamageDoneLabel.Text = String.Format(CultureConstants.DefaultCulture, DamageDoneLabel.Text, m_attacker.DamageDone,
                                                 (m_attacker.DamageDone / (double)TotalDamageDone));
        }

        /// <summary>
        /// Gets the image for the specified picture box.
        /// </summary>
        /// <param name="pictureBox">The picture box.</param>
        private void GetImageFor(PictureBox pictureBox)
        {
            string url = null;

            if (pictureBox.Equals(CharacterPictureBox))
            {
                url = String.Format(CultureConstants.InvariantCulture,
                                    NetworkConstants.CCPPortraits, m_attacker.ID, (int)EveImageSize.x64);
            }

            if (pictureBox.Equals(ShipPictureBox) || pictureBox.Equals(WeaponPictureBox))
            {
                int typeId = pictureBox.Equals(ShipPictureBox) ? m_attacker.ShipTypeID : m_attacker.WeaponTypeID;
                url = String.Format(CultureConstants.InvariantCulture,
                                    NetworkConstants.CCPIconsFromImageServer, "type", typeId, (int)EveImageSize.x32);
            }

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
