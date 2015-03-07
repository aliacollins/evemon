using System;
using EVEMon.Common;
using EVEMon.Common.Controls;

namespace EVEMon.ApiCredentialsManagement
{
    public partial class EVEMonFeaturesWindow : EVEMonForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EVEMonFeaturesWindow"/> class.
        /// </summary>
        public EVEMonFeaturesWindow()
        {
            InitializeComponent();
            BasicFeaturesLabel.Font = FontFactory.GetFont("Tahoma", 9.75F);
            AdvancedFeaturesLabel.Font = FontFactory.GetFont("Tahoma", 9.75F);
            RememberPositionKey = "EVEMonFeaturesWindow";
        }

        /// <summary>
        /// Handles the LinkClicked event of the CreateBasicAPIKeyLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void CreateBasicAPIKeyLinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(
                new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}{2}", NetworkConstants.EVECommunityBase,
                    NetworkConstants.APICredentialsBase,
                    String.Format(CultureConstants.InvariantCulture, NetworkConstants.APICredentialsPredefined,
                        (int)APIMethodsExtensions.BasicCharacterFeatures))));
        }

        /// <summary>
        /// Handles the LinkClicked event of the CreateAdvancedAPIKeyLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void CreateAllFeaturesAPIKeyLinkLabel_LinkClicked(object sender,
            System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            Util.OpenURL(
                new Uri(String.Format(CultureConstants.InvariantCulture, "{0}{1}{2}", NetworkConstants.EVECommunityBase,
                    NetworkConstants.APICredentialsBase,
                    String.Format(CultureConstants.InvariantCulture, NetworkConstants.APICredentialsPredefined,
                        (int)APIMethodsExtensions.AllCharacterFeatures))));
        }
    }
}